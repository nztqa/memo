import React from 'react';
import { BrowserRouter, Route, Link, Switch } from 'react-router-dom';
import AppContainer from '../../ui/containers/AppContainer.js';
import NotFound from '../../ui/pages/not-found/not-found.js';

export const renderRoutes = () => (
  <BrowserRouter>
    <Switch>
      <Route exact path="/" component={AppContainer} />
      <Route component={NotFound} />
    </Switch>
  </BrowserRouter>
);
