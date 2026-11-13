// © 2020 Partners HealthCare Technology & Innovation Department
import { useEffect, useMemo, useCallback } from 'react';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

export const useDebounceTime = (callback: Function, time = 500) => {
  const emitter = useMemo(() => new Subject<any>(), []);

  useEffect(() => {
    const s = emitter.pipe(debounceTime(time)).subscribe(args => callback(...args));
    return () => s.unsubscribe();
  }, [emitter, callback, time]);

  return useCallback((...args) => emitter.next(args), [emitter]);
};
