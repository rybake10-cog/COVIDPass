// © 2020 Partners HealthCare Technology & Innovation Department
import { useRef } from 'react';
import isEqual from 'react-fast-compare';

export const useDeepMemo = <T>(value: T) => {
  const ref = useRef<T>(value);
  if (!isEqual(value, ref.current)) ref.current = value;
  return ref.current;
};
