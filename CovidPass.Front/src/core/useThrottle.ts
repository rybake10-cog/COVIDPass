// © 2020 Partners HealthCare Technology & Innovation Department
import { useEffect, useMemo, useCallback } from 'react';
import { Subject } from 'rxjs';
import { throttleTime } from 'rxjs/operators';

export const useThrottle = (callback: Function, time = 10) => {
  const emitter = useMemo(() => new Subject<any>(), []);

  useEffect(() => {
    const s = emitter.pipe(throttleTime(time)).subscribe((...args) => callback(...args));
    return () => s.unsubscribe();
  }, [emitter, callback, time]);

  return useCallback((...args) => emitter.next(...args), [emitter]);
};
