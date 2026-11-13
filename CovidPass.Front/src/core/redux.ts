// © 2020 Partners HealthCare Technology & Innovation Department
import { ActionType } from 'data/actionTypes';

import { AppAction, Dictionary } from './baseTypes';

export interface AppActionCreator<T> {
  (data?: T, mod?: string, callBack?: (error?: any) => void): AppAction<T>;
  actionType: ActionType;
}

export function createAction<T>(type: ActionType): AppActionCreator<T> {
  const creator = (data: any, mod?: string, callBack?: (error?: any) => void) => ({ type, data, mod, callBack });
  creator.actionType = type;
  return creator;
}

export function createReducer<T extends { [index: string]: any }>(
  initialState: () => T,
  info: Dictionary<string | ((state: T, action: AppAction<any>) => T)>,
  supportReset = true
) {
  return (state: T = initialState(), action: AppAction<any>): T => {
    if (supportReset && action.type === ActionType.CORE_RESET_STATE) return initialState();
    const actionInfo = info[action.type];
    if (actionInfo == null) return state;
    if (typeof actionInfo === 'function') return actionInfo(state, action);
    const data = action.data === undefined ? initialState()[actionInfo] : action.data;
    if (action.mod != null) return { ...state, [actionInfo]: { ...state[actionInfo], [action.mod]: data } };
    return { ...state, [actionInfo]: data };
  };
}

export const resetState = createAction(ActionType.CORE_RESET_STATE);
