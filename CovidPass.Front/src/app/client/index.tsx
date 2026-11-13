// © 2020 Partners HealthCare Technology & Innovation Department
import React, { useCallback, useEffect } from 'react';
import { Switch, Route, Redirect } from 'react-router-dom';
import { useDispatch } from 'react-redux';

import { useMatch } from 'core/router';
import { RepeatPanel } from 'shared';
import { ActionType } from 'data/actionTypes';
import { getConfigAsync } from 'data/auth/actions';

import { Survey } from './survey/survey';
import { Application } from './application/application';

export const ClientIndex = () => {
  const match = useMatch();
  const dispatch = useDispatch();
  const checkAuth = useCallback(() => {
    dispatch(getConfigAsync());
  }, [dispatch]);
  useEffect(checkAuth, []);
  return (
    <RepeatPanel actionType={ActionType.COMMON_AUTH_GETCONFIGASYNC} action={checkAuth}>
      <Switch>
        <Route exact path={match.path + 'showApplication/:mode?'} component={Application} />
        <Route exact path={match.path + ':mode?'} component={Survey} />
        <Redirect to={match.url} />
      </Switch>
    </RepeatPanel>
  );
};
