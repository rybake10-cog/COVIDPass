// © 2020 Partners HealthCare Technology & Innovation Department
import React from 'react';
import { Route, Switch, Redirect } from 'react-router-dom';

import { asyncComponent } from 'core/asyncComponent';
import { Login } from 'app/base/login';

const ClientIndexAsync: React.FC = asyncComponent(() => import('./client').then(x => x.ClientIndex));

export const App: React.FC = () => {
  return (
    <Switch>
      <Route path="/login/:mode?" component={Login} />
      <Route path="/" component={ClientIndexAsync} />
      <Redirect to="/"></Redirect>
    </Switch>
  );
};
