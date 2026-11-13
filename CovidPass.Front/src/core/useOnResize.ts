// © 2020 Partners HealthCare Technology & Innovation Department
import { useRef, useEffect, useMemo } from 'react';
import ResizeObserverPolyfill from '@juggle/resize-observer';

const ResizeObserver = (window as any).ResizeObserver ?? ResizeObserverPolyfill;

export const useOnResize = (callback: (x: number, y: number) => any) => {
  const ref = useRef<HTMLDivElement>(null);
  const observer = useMemo(
    () =>
      new ResizeObserver((entries: any) => {
        if (entries.length > 0) {
          callback(entries[0].contentRect.width, entries[0].contentRect.height);
        }
      }),
    [callback]
  );
  useEffect(() => {
    if (ref.current) {
      observer.observe(ref.current);
      return () => observer.disconnect();
    }
  }, [observer]);
  return ref;
};
