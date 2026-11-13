// © 2020 Partners HealthCare Technology & Innovation Department
import { useLayoutEffect, useRef } from 'react';
import Popper, { PopperOptions } from 'popper.js';

export const usePopper = (options: PopperOptions = {}) => {
  const referenceRef = useRef<any>();
  const popperRef = useRef<any>();

  const popperInstanceRef = useRef<Popper>();

  useLayoutEffect(() => {
    if (referenceRef.current != null && popperRef.current != null) {
      popperInstanceRef.current = new Popper(referenceRef.current, popperRef.current, options);
    }
    return () => {
      if (popperInstanceRef.current != null) {
        popperInstanceRef.current.destroy();
        popperInstanceRef.current = undefined;
      }
    };
  }, [options]);

  useLayoutEffect(() => {
    if (popperInstanceRef.current != null) popperInstanceRef.current.update();
  }, [options]);

  return {
    reference: referenceRef,
    popper: popperRef
  };
};
