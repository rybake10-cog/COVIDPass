import './survey.scss';

import React, { useState, useCallback, useEffect } from 'react';
import { useHistory, useParams } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import { useTranslation } from 'react-i18next';

import { Flex, Field, LoadingButton, MessagesView, Input, Select, Button, LoadingSpinner } from 'shared';
import { ActionType } from 'data/actionTypes';
import { StoreType } from 'core/store';
import { submitAsync } from 'data/application/actions';
import { ApplicationSubmit } from 'data/application/models';
import { isEmpty } from 'utils/empty';
import { setAuthInfo } from 'data/auth/actions';
import { getPatientInfoAsync, setPatientInfo } from 'data/lookup/actions';
import { useDebounceTime } from 'core/useDebounceTime';
import { PatientInfo } from 'data/lookup/models';
import { useCleaning } from 'core/useCleaning';

import logoImg from './logo.png';

export const Survey: React.FC = () => {
  useCleaning(setPatientInfo);
  const history = useHistory();
  const dispatch = useDispatch();
  const { mode } = useParams<{ mode?: string }>();
  const isManual = mode?.toLocaleLowerCase() === 'manual';
  const { t, i18n } = useTranslation();
  const storedSite = localStorage.getItem('site') ?? '';

  const config = useSelector((state: StoreType) => state.auth.config);

  useEffect(() => {
    window.scrollTo(0, 0);
  }, []);

  const logout = useCallback(() => {
    dispatch(setAuthInfo());
    history.push(`/login/` + (mode ?? ''));
  }, [dispatch, history, mode]);

  const info = useSelector((state: StoreType) => state.auth.authInfo);
  const patientInfo = useSelector((state: StoreType) => state.lookup.patientInfo);
  const [message, setMessage] = useState<string>();
  const [model, setModel] = useState<ApplicationSubmit>({
    isManual,
    employeeId: '',
    login: '',
    lastName: isManual ? '' : info.lastName ?? '',
    firstName: isManual ? '' : info.firstName ?? '',
    email: isManual ? '' : info.email ?? '',
    site: config.sites.includes(storedSite) ? storedSite : '',
    initials: '',
    language: i18n.language
  });
  const [lastPatientInfo, setLastPatientInfo] = useState<PatientInfo>();
  const [searchBy, setSearchBy] = useState<string>();
  useEffect(() => {
    if (patientInfo && patientInfo !== lastPatientInfo) {
      setModel({
        ...model,
        employeeId: searchBy !== 'employeeId' ? patientInfo.employeeNumber ?? '' : model.employeeId.substring(0, 9),
        login: searchBy !== 'login' ? patientInfo.username ?? '' : model.login.substring(0, 150).trim(),
        lastName: patientInfo.lastName ?? '',
        firstName: patientInfo.firstName ?? '',
        email: patientInfo.email ?? ''
      });
      setLastPatientInfo(patientInfo);
    }
  }, [isManual, lastPatientInfo, model, patientInfo, searchBy]);
  const getPatientInfo = useDebounceTime(
    useCallback(
      (login: string, employeeId: string, field: string) => {
        if (login || employeeId) {
          setSearchBy(field);
          dispatch(getPatientInfoAsync({ login, employeeId }));
        }
      },
      [dispatch]
    ),
    1000
  );
  const onChange = (field: string, value: string) => {
    setMessage(undefined);
    if (field === 'site') localStorage.setItem('site', value);
    if (config.useLookup) {
      if (field === 'login' && value?.length >= 3) getPatientInfo(value, undefined, field);
      if (field === 'employeeId' && value?.length >= 6) getPatientInfo(undefined, value, field);
    }
    setModel({ ...model, [field]: value });
  };
  const onChangeBoolean = (field: string, value: boolean) => {
    setMessage(undefined);
    setModel({ ...model, [field]: value, noSymptoms: false });
  };

  const applicationSubmit = useCallback(() => {
    if (isEmpty(model.site)) {
      setMessage(`${t('surveyPage.provideValue')} ${t('surveyPage.site')}`);
      return;
    }
    if (isManual) {
      if (isEmpty(model.lastName)) {
        setMessage(`${t('surveyPage.provideValue')} ${t('surveyPage.lastName')}`);
        return;
      }
      if (isEmpty(model.firstName)) {
        setMessage(`${t('surveyPage.provideValue')} ${t('surveyPage.firstName')}`);
        return;
      }
    } else {
      if (model.noSymptoms === true && isEmpty(model.initials)) {
        setMessage(`${t('surveyPage.provideValue')} ${t('surveyPage.initials')}`);
        return;
      }
    }
    if (
      model.noSymptoms !== true &&
      model.muscleAche !== true &&
      model.fever !== true &&
      model.cough !== true &&
      model.shortnessOfBreath !== true &&
      model.soreThroat !== true &&
      model.newLossOfSmell !== true &&
      model.runnyNose !== true
    ) {
      setMessage(`${t('surveyPage.provideValue')} ${t('surveyPage.symptoms')}`);
      return;
    }
    setMessage(undefined);
    dispatch(
      submitAsync(model, undefined, error => {
        if (!error) history.push('/showApplication/' + (mode ?? ''));
      })
    );
  }, [dispatch, history, isManual, mode, model, t]);

  return (
    <Flex justifyContent="center" className="survey">
      <Flex className="survey__container" direction="column">
        <Flex alignItems="center" justifyContent="center">
          <img src={logoImg} alt="logo" />
        </Flex>
        {isManual && (
          <>
            <div className="survey__hellotext">{t('surveyPage.titleForManualMode')}</div>
            <div>{t('surveyPage.descriptionForManualMode')}</div>
          </>
        )}
        {!isManual && (
          <>
            <div className="survey__hellotext">{t('surveyPage.titleForSelfService')}</div>
            <div>{t('surveyPage.descriptionForSelfService')}</div>
          </>
        )}
        <Flex className="survey_fields" direction="column">
          {isManual && (
            <>
              <Field text={t('surveyPage.employeeId')} view="responsive-row">
                <Input
                  autoFocus
                  type="text"
                  value={model.employeeId}
                  onChange={x => onChange('employeeId', x.target.value.trim())}
                />
                {searchBy === 'employeeId' && (
                  <LoadingSpinner actionType={ActionType.LOOKUP_GETPATIENTINFOASYNC}></LoadingSpinner>
                )}
              </Field>
              <Flex className="survey__orPanel" justifyContent="center">
                {t('surveyPage.or')}
              </Flex>
              <Field text={t('surveyPage.phsUserId')} view="responsive-row">
                <Input type="text" value={model.login} onChange={x => onChange('login', x.target.value.trim())} />
                {searchBy === 'login' && (
                  <LoadingSpinner actionType={ActionType.LOOKUP_GETPATIENTINFOASYNC}></LoadingSpinner>
                )}
              </Field>
              <Field
                text={
                  <span>
                    <span className="red">*</span> {t('surveyPage.lastName')}
                  </span>
                }
                view="responsive-row">
                <Input
                  type="text"
                  value={model.lastName}
                  onChange={x => onChange('lastName', x.target.value.trimStart())}
                />
              </Field>
              <Field
                text={
                  <span>
                    <span className="red">*</span> {t('surveyPage.firstName')}
                  </span>
                }
                view="responsive-row">
                <Input
                  type="text"
                  value={model.firstName}
                  onChange={x => onChange('firstName', x.target.value.trimStart())}
                />
              </Field>
              <Field text={t('surveyPage.email')} view="responsive-row">
                <Input type="text" value={model.email} onChange={x => onChange('email', x.target.value.trim())} />
              </Field>
            </>
          )}
          {!isManual && (
            <>
              <Field text={t('surveyPage.phsUserId')} view="responsive-row">
                <div className="staticFieldValue">{info.login}</div>
              </Field>
              <Field text={t('surveyPage.lastName')} view="responsive-row">
                <div className="staticFieldValue">{info.lastName}</div>
              </Field>
              <Field text={t('surveyPage.firstName')} view="responsive-row">
                <div className="staticFieldValue">{info.firstName}</div>
              </Field>
              <Field text={t('surveyPage.email')} view="responsive-row">
                <div className="staticFieldValue">{info.email}</div>
              </Field>
            </>
          )}
          {config.sites && (
            <Field
              text={
                <span>
                  <span className="red">*</span> {t('surveyPage.site')}
                </span>
              }
              view="responsive-row">
              <Select value={model.site} onChange={x => onChange('site', x.target.value)}>
                <option value=""></option>
                {model.site && !config.sites.includes(model.site) && <option value={model.site}>{model.site}</option>}
                {config.sites.map(site => (
                  <option key={site} value={site}>
                    {site}
                  </option>
                ))}
              </Select>
            </Field>
          )}
          <Field text={t('surveyPage.symptomsQuestion')} view="responsive-row" className="placeOnTop">
            <Flex direction="column" className="checkboxContainer">
              <Button view={model.fever ? 'red' : 'clear'} onClick={() => onChangeBoolean('fever', !model.fever)}>
                {t('surveyPage.fever')}
              </Button>
              <Button
                view={model.soreThroat ? 'red' : 'clear'}
                onClick={() => onChangeBoolean('soreThroat', !model.soreThroat)}>
                {t('surveyPage.soreThroat')}
              </Button>
              <Button view={model.cough ? 'red' : 'clear'} onClick={() => onChangeBoolean('cough', !model.cough)}>
                {t('surveyPage.cough')}
              </Button>
              <Button
                view={model.runnyNose ? 'red' : 'clear'}
                onClick={() => onChangeBoolean('runnyNose', !model.runnyNose)}>
                {t('surveyPage.runnyNose')}
              </Button>
              <Button
                view={model.muscleAche ? 'red' : 'clear'}
                onClick={() => onChangeBoolean('muscleAche', !model.muscleAche)}>
                {t('surveyPage.muscleAche')}
              </Button>
              <Button
                view={model.newLossOfSmell ? 'red' : 'clear'}
                onClick={() => onChangeBoolean('newLossOfSmell', !model.newLossOfSmell)}>
                {t('surveyPage.newLossOfSmell')}
              </Button>
              <Button
                view={model.shortnessOfBreath ? 'red' : 'clear'}
                onClick={() => onChangeBoolean('shortnessOfBreath', !model.shortnessOfBreath)}>
                {t('surveyPage.shortnessOfBreath')}
              </Button>
              <Button
                view={model.noSymptoms ? 'green' : 'clear'}
                onClick={() => {
                  setMessage(undefined);
                  setModel({
                    ...model,
                    noSymptoms: !model.noSymptoms,
                    cough: false,
                    fever: false,
                    shortnessOfBreath: false,
                    soreThroat: false,
                    newLossOfSmell: false,
                    runnyNose: false,
                    muscleAche: false
                  });
                }}>
                {t('surveyPage.noSymptoms')}
              </Button>
            </Flex>
          </Field>
        </Flex>
        <Flex className="survey__resultInfo">
          {model.noSymptoms === true && (
            <div className="message">
              <p>{t('surveyPage.noSymptoms_text1')}</p>
              <p className="large">{t('surveyPage.noSymptoms_text2')}</p>
              {!isManual && (
                <>
                  <p>
                    {t('surveyPage.noSymptoms_text3')}
                    <br />
                    {t('surveyPage.noSymptoms_text4')}
                  </p>
                  <p>
                    <Input
                      type="text"
                      className="initials"
                      value={model.initials}
                      onChange={x => onChange('initials', x.target.value.substring(0, 5).trim())}
                    />
                  </p>
                </>
              )}
            </div>
          )}
          {(model.influenza === true ||
            model.fever === true ||
            model.cough === true ||
            model.shortnessOfBreath === true ||
            model.soreThroat === true ||
            model.diarrheaNausea === true ||
            model.newLossOfSmell === true ||
            model.runnyNose === true ||
            model.muscleAche === true) && (
            <div className="message">
              <p>{t('surveyPage.symptoms_text1')}</p>
              <p className="large">{t('surveyPage.symptoms_text2')}</p>
              <p className="medium">
                {t('surveyPage.symptoms_text3')} <br /> <a href="tel:+16177248100">617-724-8100</a>
              </p>
              <p className="small">{t('surveyPage.symptoms_text4')} 603-740-2800</p>
              <p className="medium">{t('surveyPage.symptoms_text5')}</p>
            </div>
          )}
        </Flex>
        <Flex justifyContent="center" alignItems="center" className="survey__bluePanel">
          <div>
            {t('surveyPage.pressSubmit')}
            {model.lastName !== '' && model.firstName !== ''
              ? ' ' + t('surveyPage.for') + ' ' + model.lastName + ', ' + model.firstName
              : ''}
          </div>
        </Flex>
        <MessagesView messages={message} actionType={ActionType.APPLICATION_SUBMITASYNC}></MessagesView>
        <Flex className="buttonsPanel" justifyContent="center">
          <Button view="link" onClick={logout}>
            {t('common.logout')}
          </Button>
          <LoadingButton actionType={ActionType.APPLICATION_SUBMITASYNC} onClick={applicationSubmit}>
            {t('surveyPage.submit')}
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
