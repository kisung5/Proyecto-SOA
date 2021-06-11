import * as React from "react";
import { Component } from 'react';
import { w3cwebsocket as W3CWebSocket } from "websocket";
import { Card, Input} from 'antd';
import 'antd/dist/antd.css';
import Button from "@material-ui/core/Button";
import { makeStyles } from '@material-ui/core/styles';
import LinearProgress, { LinearProgressProps } from '@material-ui/core/LinearProgress';
import Typography from '@material-ui/core/Typography';
import Box from '@material-ui/core/Box';
import { useState, useCallback, useMemo, useRef } from 'react';
import useWebSocket, { ReadyState } from 'react-use-websocket';

const { Search } = Input;
const { Meta } = Card;

const client = new W3CWebSocket('ws://127.0.0.1:8000/ws');

interface AbcState {
  items: any[]; //replace any with suitable type
  searchVal: string;
  messages: any[];
  userName: string;
  isLoggedIn: boolean;
}

const useStyles = makeStyles({
  root: {
    width: '100%',
  },
});

export default class LoadingFile extends Component<{}, AbcState> {

  onButtonClicked = (value) => {
    client.send(JSON.stringify({
      type: "message",
      msg: value,
      user: this.state.userName
    }));
    this.setState({ searchVal: '' })
  }

  //Progress bar
  LoadingFileX = (props: LinearProgressProps & { value: number }) => {
    return (
      <Box display="flex" alignItems="center">
        <Box width="100%" mr={1}>
          <LinearProgress variant="determinate" {...props} />
        </Box>
        <Box minWidth={35}>
          <Typography variant="body2" color="textSecondary">{`${Math.round(
            props.value,
          )}%`}</Typography>
        </Box>
      </Box>
    );
  }
  //Progress bar
  LinearWithValueLabel = () => {
    const classes = useStyles();
    const [progress, setProgress] = React.useState(10);
  
    React.useEffect(() => {
      const timer = setInterval(() => {
        setProgress((prevProgress) => (prevProgress >= 100 ? 100 : prevProgress + 10));
      }, 800);
      return () => {
        clearInterval(timer);
      };
    }, []);
  
    return (
      <div className={classes.root}>
        <h1>Analysis Results</h1>
        <this.LoadingFileX value={progress} />
      </div>
    );
  }
  render() {
    return (
      <>
      <this.LinearWithValueLabel />
        </>
    )
  }
}

