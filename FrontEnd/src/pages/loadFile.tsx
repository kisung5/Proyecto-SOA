import * as React from "react";
import { Component } from 'react';
import { Container, createMuiTheme, CssBaseline, Box, Typography } from '@material-ui/core'
import { ThemeProvider } from "@material-ui/styles";
import { hasCookie, DeleteCookie } from '../Utility/CookieManager'
import { messageHandler, logoutHandler } from '../Utility/MessageHandler'
import 'antd/dist/antd.css';
import LinearProgress from '@material-ui/core/LinearProgress';
import { makeStyles } from '@material-ui/core/styles';

var client = new WebSocket('ws://localhost:5000/ws')  
const theme = createMuiTheme({
  palette: {
    type: "dark"
  }
});


const useStyles = makeStyles({
  root: {
    width: '100%',
  },
});

export default class LoadFile extends Component{
  incomingMessageListener(incomingMessageListener: any) {
    throw new Error("Method not implemented.");
  }
 
  
  state = {
    userName: null,
    userColor: null,
    ws: null,
    disconnected: true,
    logout: false,
    formFields: [],
    userList : [],
    newOpen: false,
  }
  
  handleLogout = () => {
    let obj = hasCookie()
    logoutHandler(obj.entryToken)
    DeleteCookie(['entryToken'])
    this.setState({
      logout: true
    })
  }
  handleNewDialogClose = () => {
    this.setState({
      newOpen: false
    })
  }

  handleFormHistory = (formData) => {
    console.log("To update the forms: ", formData)
    this.setState({
      formFields: formData
    })
  }
  openEventListener = (event) => {
    let obj = hasCookie()
    this.setState({ws: client, disconnected: false})
    console.log('Websocket Client Connected')
    client.send(JSON.stringify({
      messageType: 'room entry',
      entryToken: obj.entryToken
    }))
  }

  closeSocket = (event) => {
    console.log("You are disconnected")
    this.setState({disconnected: true})
    setTimeout(()=>{
      if (!this.state.logout) {
        console.log("Retrying connection")
        client = new WebSocket('ws://localhost:5000/ws')
        client.addEventListener('open', this.openEventListener)
        client.addEventListener('close', this.closeSocket)
      }
    }, 5000)
  }
  componentDidMount() {
    client.addEventListener('open', this.openEventListener)
    client.addEventListener('close', this.closeSocket)
    console.log("in component mount");
    // get information from user
  }

  stateUpdateMount = (userName, colour) => {
    this.setState({
      userName: userName,
      userColor: colour,
      logout: false
    })
  }

  //Progress bar
  LoadingFileX = (props) => {
    return (React.createElement(Box, { display: "flex", alignItems: "center" },
      React.createElement(Box, { width: "100%", mr: 1 },
      React.createElement(LinearProgress, Object.assign({ variant: "determinate" }, props))),
      React.createElement(Box, { minWidth: 35 },
      React.createElement(Typography, { variant: "body2", color: "textSecondary" }, `${Math.round(props.value)}%`))));
  };

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
    const items = this.state.formFields.map((item, i) => 
      <div key={i}>
        {!item.IsDeleted}
        <br></br>
      </div>
    )
    return ( 
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <div className="App">
          <Box>
            <this.LinearWithValueLabel/>
            {!this.state.logout && 
            <Container>
              <div style={{ justifyContent:'center', display:'flex'}}>
              </div>
            </Container>
            }
          </Box>
          {!this.state.logout && 
          <Container style={{marginTop:"50px", textAlign:'center'}}>
    
              <Container style={{marginTop:"20px"}}>
                {items}
              </Container>
          </Container>
          }
        </div>
      </ThemeProvider>
    )
  }
}


