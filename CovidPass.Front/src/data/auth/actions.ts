// © 2020 Partners HealthCare Technology & Innovation Department
import { createAction } from 'core/redux';
import { ActionType } from 'data/actionTypes';

import { AuthInfo, LoginModel, Config } from './models';

export const setAuthInfo = createAction<AuthInfo>(ActionType.COMMON_AUTH_SETAUTHINFO);
export const setForm = createAction<LoginModel>(ActionType.COMMON_AUTH_SETFORM);
export const loginAsync = createAction<LoginModel>(ActionType.COMMON_AUTH_LOGINASYNC);
export const getConfigAsync = createAction(ActionType.COMMON_AUTH_GETCONFIGASYNC);
export const setConfig = createAction<Config>(ActionType.COMMON_AUTH_SETCONFIG);
