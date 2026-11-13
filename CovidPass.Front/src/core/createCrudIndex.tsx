// © 2020 Partners HealthCare Technology & Innovation Department
import React from 'react';
import { Switch, Route, RouteComponentProps, Redirect } from 'react-router-dom';

export const createCrudIndex = (one: React.ComponentType, list: React.ComponentType) => ({
  match
}: RouteComponentProps) => (
  <Switch>
    <Route exact path={`${match.url}/add`} component={one} />
    <Route exact path={`${match.url}/:id`} component={one} />
    <Route exact path={match.url} component={list} />
    <Redirect to={match.url} />
  </Switch>
);
