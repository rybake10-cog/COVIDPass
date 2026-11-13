// © 2020 Partners HealthCare Technology & Innovation Department
import { useEffect } from 'react';
import { useDispatch } from 'react-redux';

import { AppAction } from './baseTypes';

export const useCleaning = (actionCreator: () => AppAction<any>) => {
  const dispatch = useDispatch();
  useEffect(() => {
    return () => {
      dispatch(actionCreator());
    };
  }, [dispatch, actionCreator]);
};
