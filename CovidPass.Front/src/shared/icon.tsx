// © 2020 Partners HealthCare Technology & Innovation Department
import { library } from '@fortawesome/fontawesome-svg-core';
//solid
import { faCircle } from '@fortawesome/free-solid-svg-icons/faCircle';
import { faSpinner } from '@fortawesome/free-solid-svg-icons/faSpinner';
import { faTimes } from '@fortawesome/free-solid-svg-icons/faTimes';
//regular
//import { faCircle } from '@fortawesome/free-regular-svg-icons/faCircle';
//
import React from 'react';
import classNames from 'classnames';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

library.add(
  //solid
  faSpinner,
  faCircle,
  faTimes
  //regular
  //faCircle
);

export type ImportedIcon = 'spinner' | 'circle' | 'times';
export interface Props extends React.HTMLAttributes<any> {
  className?: string;
  spin?: boolean;
  prefix?: 'fas' | 'far';
  name: ImportedIcon;
}

export const Icon: React.FC<Props> = ({ prefix = 'fas', name, spin, className, ...other }) => {
  const classes = classNames('icon-container', className);
  return <FontAwesomeIcon icon={[prefix, name]} spin={spin} className={classes} {...other}></FontAwesomeIcon>;
};
