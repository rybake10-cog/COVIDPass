// © 2020 Partners HealthCare Technology & Innovation Department
import { createReducer } from 'core/redux';
import { ActionType } from 'data/actionTypes';

import { LoginModel, AuthInfo, Config } from './models';

interface AuthInitialState {
  form: LoginModel;
  authInfo: AuthInfo;
  config: Config;
}

const initialState: () => AuthInitialState = () => ({
  authInfo: {
    isAuth: false
  },
  form: {
    login: '',
    password: ''
  },
  config: {
    useLookup: false,
    sites: []
  }
});

export const authReducer = createReducer(initialState, {
  [ActionType.COMMON_AUTH_SETFORM]: 'form',
  [ActionType.COMMON_AUTH_SETCONFIG]: 'config',
  [ActionType.COMMON_AUTH_SETAUTHINFO]: (state, action) => {
    if (action.data == null) localStorage.removeItem('authInfo');
    else localStorage.setItem('authInfo', JSON.stringify(action.data));
    return {
      ...state,
      authInfo: action.data == null ? initialState().authInfo : action.data
    };
  }
});
