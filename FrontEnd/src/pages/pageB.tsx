import * as React from "react";
import { useState, useEffect } from 'react';
import * as Path from 'path';
import uploadFileToBlob, { isStorageConfigured } from './azure-storage-blob';
import { useHistory } from "react-router-dom";
import { makeStyles } from '@material-ui/core/styles';
import { Grid } from '@material-ui/core';
import CircularProgress from '@material-ui/core/CircularProgress';
import Keycloak from 'keycloak-js';

// Import React Table
import ReactTable from "react-table-6";
import "react-table-6/react-table.css"
import { Button } from "@material-ui/core";

const storageConfigured = isStorageConfigured();

declare global { 
  interface Window{
     msCrypto: Crypto;
  }
}

export const PageB = (): JSX.Element => {
  // all blobs in container
  const [blobList, setBlobList] = useState<string[]>([]);
  const history = useHistory();
  // current file to upload into contobList] = useStainer
  const [fileSelected, setFileSelected] = useState(null);

  // UI/form management
  const [uploading, setUploading] = useState(false);

  // === Client side ===
  const crypto = window.crypto || window.msCrypto;
  var array = new Uint32Array(1);
  crypto.getRandomValues(array); // Compliant for security-sensitive use cases

  const [inputKey, setInputKey] = useState(array[0].toString(36));
  const useStyles = makeStyles({
    table: {
      minWidth: 650,
    },
  });

  const classes = useStyles();

  const onFileChange = (event: any) => {
    // capture file into state
    setFileSelected(event.target.files[0]);
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
    crypto.getRandomValues(array); // Compliant for security-sensitive use cases
    setInputKey(array[0].toString(36));
  };

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
          onClick={() => {history.push('/LoadingFile') }}
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

  function createData(name: string, calories: number, fat: number, carbs: number, protein: number) {
    return { name, calories, fat, carbs, protein };
  }
  const rows = [
    createData('Frozen yoghurt', 159, 6.0, 24, 4.0),
    createData('Ice cream sandwich', 237, 9.0, 37, 4.3),
    createData('Eclair', 262, 16.0, 24, 6.0),
  ];
  
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
                   Cell: row => (
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
 
                   Cell: row => (
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


