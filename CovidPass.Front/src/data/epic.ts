// © 2020 Partners HealthCare Technology & Innovation Department
import { combineEpics } from 'redux-observable';

import { routerPush, routerGoBack } from 'core/epic';

import { authEpic } from './auth/epic';
import { applicationEpic } from './application/epic';
import { lookupEpic } from './lookup/epic';

export const rootEpic = combineEpics(routerPush, routerGoBack, authEpic, applicationEpic, lookupEpic);
