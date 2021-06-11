import * as React from "react";
import makeStyles from "@material-ui/styles/makeStyles";
import createStyles from "@material-ui/styles/createStyles";
import TextField from "@material-ui/core/TextField";
import { LoginEntity, createEmptyLogin } from "../model/login";
import { RegisterEntity, createEmptyReg } from "../model/register_form";
import { useHistory } from "react-router-dom";
import 'antd/dist/antd.css';
import 'antd/dist/antd.css';
import Button from "@material-ui/core/Button";
import axios from 'axios';
import {useState} from 'react';
import { setUserSession } from './common';

interface PropsForm {
  onLogin: (login: LoginEntity) => void;
}

// https://material-ui.com/styles/api/#makestyles-styles-options-hook
const useFormStyles = makeStyles((theme) =>
  createStyles({
    formContainer: {
      display: "flex",
      flexDirection: "column",
      justifyContent: "center",
    },
  })
);

export const LoginComponent: React.FC<PropsForm> = (props) => {
  const { onLogin} = props;
  const history = useHistory();
  const [loginInfo, setLoginInfo] = React.useState<LoginEntity>(
    createEmptyLogin()
  );
  const [regInfo, setRegInfo] = React.useState<RegisterEntity>(
    createEmptyReg()
  );
  const [loading, setLoading] = useState(false);
  const username = useFormInput('');
  const password = useFormInput('');
  const [error, setError] = useState(null);
  const classes = useFormStyles();
  const onTexFieldChange = (fieldId) => (e) => {
    setLoginInfo({
      ...loginInfo,
      [fieldId]: e.target.value,
    });
  };

  const handleLogin = () => {
    setError(null);
    setLoading(true);
    axios.post('http://localhost:4000/users/signin', { username: username.value, password: password.value }).then(response => {
      setLoading(false);
      setUserSession(response.data.token, response.data.user);
      history.push('/Register');
    }).catch(error => {
      setLoading(false);
      if (error.response.status === 401) setError(error.response.data.message);
      else setError("Something went wrong. Please try again later.");
    });
  }
  return (
    <div className={classes.formContainer}>
      <TextField
        label="Email"
        margin="normal"
        value={loginInfo.login}
        onChange={onTexFieldChange("login")}
      />
      <TextField
        label="Password"
        type="password"
        margin="normal"
        value={loginInfo.password}
        onChange={onTexFieldChange("password")}
      />
      <Button
        variant="contained"
        color="primary"
        onClick={() => onLogin(loginInfo)}
      >
        Login
      </Button>

      <p></p>

      <Button
        variant="contained"
        color="primary"
        onClick={() => {history.push('/Register') }}
      >
        Register Now
      </Button>
    </div>
  );
};


const useFormInput = initialValue => {
  const [value, setValue] = useState(initialValue);
 
  const handleChange = e => {
    setValue(e.target.value);
  }
  return {
    value,
    onChange: handleChange
  }
}
