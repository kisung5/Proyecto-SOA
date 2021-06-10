import * as React from "react";
import { Component } from 'react';
import * as ReactDOM from "react-dom";
import { w3cwebsocket as W3CWebSocket } from "websocket";
import { Card, Avatar, Input, Typography } from 'antd';
import 'antd/dist/antd.css';
import Button from "@material-ui/core/Button";

const { Search } = Input;
const { Text } = Typography;
const { Meta } = Card;

const client = new W3CWebSocket('ws://127.0.0.1:8000');

interface AbcState {
  items?: any[]; //replace any with suitable type
  searchVal?: string;
  messages?: any[];
  userName?: string;
}

export default class LoadingFile extends Component<{}, AbcState> {

  onButtonClicked = (value: string) => {
    client.send(JSON.stringify({
      type: "message",
      msg: value,
      user: this.state.userName
    }));
    this.setState({ searchVal: '' })
  }
  componentDidMount() {
    client.onopen = () => {
      console.log('WebSocket Client Connected');
    };
    client.onmessage = (message) => {
      const dataFromServer = JSON.parse(message.data);
      console.log('got reply! ', dataFromServer);
      if (dataFromServer.type === "message") {
        this.setState((state) =>
          ({
            messages: [...state.messages,
            {
              msg: dataFromServer.msg,
              user: dataFromServer.user
            }]
          })
        );
      }
    };
  }
  render() {
    
    return (
      <Button type="submit">
        Upload file
          </Button>
    )
  }
}

