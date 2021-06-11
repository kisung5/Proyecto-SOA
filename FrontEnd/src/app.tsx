import * as React from "react";
import Keycloak, { KeycloakAdapter, KeycloakInstance } from 'keycloak-js';
import { Component } from 'react';
import {  HashRouter, Switch, Route, Redirect } from "react-router-dom";
import { PageB } from "./pages/pageB";
import LoadFile from './pages/loadFile';

interface IState {
  keycloak: KeycloakInstance; //replace any with suitable type
  authenticated: boolean;
}

export default class App extends Component<{}, IState> {
  constructor(props) {
    super(props);
    this.state = { keycloak: null, authenticated: false };
  }

  componentDidMount() {
    const keycloak = Keycloak('./keycloak.json');
    keycloak.init({onLoad: 'login-required'}).success(authenticated => {
      this.setState({ keycloak: keycloak, authenticated: authenticated })
    });

    //store authentication tokens in sessionStorage for usage in app
    sessionStorage.setItem('authentication', keycloak.token);
    sessionStorage.setItem('refreshToken', keycloak.refreshToken);
  }
  
  render() {
    if (this.state.keycloak) {
      if (this.state.authenticated) return (
        <HashRouter>
          <Switch>
            <Route exact path="/">
              <Redirect to="/home" />
            </Route>
            <Route path="/home" component={PageB} />
            <Route path="/loadFile" component={LoadFile} />
          </Switch>
        </HashRouter>
      );
      else return (<div>Unable to authenticate!</div>);
    }
    else return (
      <div>Initializing Keycloak...</div>
    );
  }
};
