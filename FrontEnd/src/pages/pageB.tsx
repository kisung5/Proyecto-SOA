import * as React from "react";
import { useState, useEffect } from 'react';
import * as Path from 'path';
import uploadFileToBlob, { isStorageConfigured } from './azure-storage-blob';
import { useHistory } from "react-router-dom";
import { Grid } from '@material-ui/core';
import CircularProgress from '@material-ui/core/CircularProgress';
import Keycloak from 'keycloak-js';
import axios from 'axios';
import { setUserSession } from './common';
// Import React Table
import ReactTable from "react-table-6";
import "react-table-6/react-table.css"
import { Button } from "@material-ui/core";

const storageConfigured = isStorageConfigured();

export const PageB = (): JSX.Element => {
  // all blobs in container
  const [blobList, setBlobList] = useState<string[]>([]);
  const history = useHistory();
  // current file to upload into contobList] = useStainer
  const [fileSelected, setFileSelected] = useState(null);
  // UI/form management
  const [uploading, setUploading] = useState(false);
  const [inputKey, setInputKey] = useState(Math.random().toString(36));

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const onFileChange = (event: any) => {
    // capture file into state
    setFileSelected(event.target.files[0]);
    console.log("Message" + event.target.files[0]);
    setError(null);
    setLoading(true);
    axios.post('https://localhost:44301/Analysis', { type: 4, fileName: event.target.files[0].name}).then(response => {
      setLoading(false);
      setUserSession(response.data.token, response.data.user);
    }).catch(error => {
      setLoading(false);
        if (error.response.status === 401) setError(error.response.data.message);
        else setError("Something went wrong. Please try again later.");
      });
  };

  const onFileUpload = async () => {
    // prepare UI
    setUploading(true);

    // *** UPLOAD TO AZURE STORAGE ***
    const blobsInContainer: string[] = await uploadFileToBlob(fileSelected);

    // prepare UI for results
    setBlobList(blobsInContainer);

    // reset state/form
    setFileSelected(null);
    setUploading(false);
    setInputKey(Math.random().toString(36));
  };

      //API Post. Sends file name
/*   const handleLogin = () => {
    setError(null);
    setLoading(true);
    axios.post('https://localhost:44301/Analysis', { type: 4, fileName: FileName}).then(response => {
      setLoading(false);
      setUserSession(response.data.token, response.data.user);
      //history.push('/Register');
    }).catch(error => {
      setLoading(false);
        if (error.response.status === 401) setError(error.response.data.message);
        else setError("Something went wrong. Please try again later.");
      });
    } */

  const onLogout = async () => {
    const keycloak = Keycloak();
    keycloak.logout({ redirectUri : "http://localhost:3000/" });
  };

  // display form
  const DisplayForm = () => (
    <Grid container> 
      <Grid container item xs={4} justify="flex-start">
        <Button variant="contained" component="label">
          <input type="file" onChange={onFileChange} key={inputKey || ''} />
        </Button>
        
        <Button type="submit" onClick={onFileUpload}>
          Upload file
        </Button>
      </Grid>

      <Grid container item xs={4} justify="center">
        <Button
          variant="contained"
          color="primary"
          onClick={() => {history.push('/loadFile') }}
        >
          Analyze document
        </Button>
      </Grid>
      

      <Grid container item xs={3} justify="flex-end">
        <Button onClick={onLogout} variant="outlined" color="secondary">
          Logout
        </Button>
      </Grid>
      <Grid item xs={1}></Grid>
    </Grid>
  )
  
  // display file name and image
  const DisplayImagesFromContainer = () => (
    <div>
      <h2>Container items</h2>
      <ul>
      <ReactTable
           data={blobList.map((item) => {
            return (
              <li key={item}>
              </li>
            );
          })}
           columns={[
             {
               Header: "File Name",
               columns: [
                 {
                   accessor: "fileName",
                   Cell: () => (
                     <div style={{ textAlign: "right" }}>{blobList.map((item) => {
                      return (
                        <li key={item}>
                          <div>
                            {Path.basename(item)}
                          </div>
                        </li>

                      );
                    })}</div>
                   )
                 },
               ]
             },
             {
               Header: "File Status",
               columns: [
                 {
                   accessor: "status",
 
                   Cell: () => (
                     <div style={{ textAlign: "center" }}>{"Uploaded"}</div>
                   )
                 },
               ]
             },
           ]}
           defaultPageSize={1}
           className="-striped -highlight"
         />
      </ul>
    </div>
  );
  
  return (
    <div>
      <h1>Please select a .PDF, TXT or Word file</h1>
      {storageConfigured && !uploading && DisplayForm()}
      {storageConfigured && uploading && <CircularProgress color="secondary" />}
      <hr />
      {storageConfigured && blobList.length > 0 && DisplayImagesFromContainer()}
      {!storageConfigured && <div>Storage is not configured.</div>}
    </div>
  );
};



