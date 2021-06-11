import * as React from "react";
import { HashRouter, Switch, Route } from "react-router-dom";
import { LoginContainer } from "./pages/login.container";
import { PageB } from "./pages/pageB";
import Register from "./pages/register";
import LoadFile from './pages/loadFile';

export const AppS = () => {
  
  return (
    <>
      <HashRouter>
        <Switch>
          <Route exact={true} path="/" component={LoginContainer} />
          <Route path="/pageB" component={PageB} />
          <Route path="/register" component={Register} />
          <Route path="/loadFile" component={LoadFile} />
        </Switch>
      </HashRouter>
    </>
  );
};
