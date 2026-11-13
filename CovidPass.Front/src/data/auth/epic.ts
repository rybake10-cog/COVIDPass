// © 2020 Partners HealthCare Technology & Innovation Department
import { of } from 'rxjs';
import { mergeMap, map } from 'rxjs/operators';
import { combineEpics } from 'redux-observable';

import { createEpic } from 'core/epic';

import { setAuthInfo, setForm, loginAsync, getConfigAsync, setConfig } from './actions';
import { login, getConfig } from './api';

const loginEpic = createEpic(loginAsync, data =>
  login(data).pipe(mergeMap(response => of(setAuthInfo(response), setForm())))
);

const getConfigEpic = createEpic(getConfigAsync, () =>
  getConfig().pipe(
    map(response => {
      response.sites?.sort((a, b) => (a > b ? 1 : a == b ? 0 : -1));
      return setConfig(response);
    })
  )
);

export const authEpic = combineEpics(loginEpic, getConfigEpic);
