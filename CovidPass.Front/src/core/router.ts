// © 2020 Partners HealthCare Technology & Innovation Department
import { __RouterContext, RouteComponentProps } from 'react-router';
import { useContext, useMemo } from 'react';
import { parse, ParsedQuery } from 'query-string';
import { Location } from 'history';

import { ActionType } from 'data/actionTypes';

import { createAction } from './redux';

// The react-router guys say that this will be what the official API probably looks like
// https://github.com/ReactTraining/react-router/pull/6453#issuecomment-474600561

export const useLocation = <P = {}>() => {
  const context = useContext(__RouterContext) as RouteComponentProps<P>;
  return context.location;
};

export const useLocationParams = <P extends ParsedQuery<string | number | boolean>>() => {
  const context = useContext(__RouterContext);
  return useMemo(
    () =>
      parse(context.location.search, {
        parseBooleans: true,
        parseNumbers: true
      }) as P,
    [context.location.search]
  );
};

export function useMatch<P = {}>() {
  const context = useContext(__RouterContext) as RouteComponentProps<P>;
  return context.match;
}

export function useMatchParams<P = {}>() {
  const context = useContext(__RouterContext) as RouteComponentProps<P>;
  return context.match.params;
}

export const pushRoute = createAction<string | Location>(ActionType.CORE_ROUTER_PUSH);
export const goBackRoute = createAction(ActionType.CORE_ROUTER_GOBACK);
