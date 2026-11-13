import './login.scss';

import React, { useEffect, useCallback } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { useHistory, useParams } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

import { StoreType } from 'core/store';
import { resetState } from 'core/redux';
import { Flex, Input, LoadingButton, MessagesView, Field, Select } from 'shared';
import { ActionType } from 'data/actionTypes';
import { loginAsync, setForm } from 'data/auth/actions';

import logoImg from './Your_Logo_Here.jpg';

export const Login: React.FC = () => {
  const { t, i18n } = useTranslation();
  const changeLang = useCallback(
    (lang: string) => {
      i18n.changeLanguage(lang);
      localStorage.setItem('lang', lang);
    },
    [i18n]
  );
  const history = useHistory();
  const dispatch = useDispatch();
  const { mode } = useParams<{ mode?: string }>();
  const form = useSelector((state: StoreType) => state.auth.form);
  const onChange = (field: string, value: string) => dispatch(setForm({ ...form, [field]: value.trim() }));
  const handleKeyDown = (e: React.KeyboardEvent<any>) => {
    e.key === 'Enter' && login();
  };

  useEffect(() => {
    dispatch(resetState());
  }, [dispatch]);

  const login = useCallback(() => {
    dispatch(
      loginAsync(form, undefined, error => {
        if (!error) history.push('/' + (mode ?? ''));
      })
    );
  }, [dispatch, form, history, mode]);

  return (
    <Flex justifyContent="center" alignItems="center" className="loginPage">
      <Flex direction="column" className="loginPage__form">
        <img src={logoImg} alt="logo" />
        <div className="loginPage__text">
          <p>{t('loginPage.forEmployeeOnly')}</p>
          <p>{t('loginPage.mainText')}</p>
          <p>{t('loginPage.useCredentials')}</p>
        </div>

        <MessagesView actionType={ActionType.COMMON_AUTH_LOGINASYNC} />
        <Field text={t('loginPage.username')} view="row" justifyContent="center">
          <Input type="text" value={form.login} onChange={x => onChange('login', x.target.value)} />
        </Field>
        <Field text={t('loginPage.password')} view="row" justifyContent="center">
          <Input
            type="password"
            value={form.password}
            onChange={x => onChange('password', x.target.value)}
            onKeyDown={e => handleKeyDown(e)}
          />
        </Field>
        <Flex justifyContent="center">
          <Select className="langSelector" value={i18n.language} onChange={x => changeLang(x.target.value)}>
            <option value="en">English</option>
            <option value="es">Español</option>
            <option value="ht">Kreyol</option>
          </Select>
          <LoadingButton actionType={ActionType.COMMON_AUTH_LOGINASYNC} onClick={login}>
            {t('loginPage.login')}
          </LoadingButton>
        </Flex>
        <div>
          <footer>
            <p className="footerStyle">&#169; Copyright Partners HealthCare Innovation and Technology Department.</p>
          </footer>
        </div>
      </Flex>
    </Flex>
  );
};
