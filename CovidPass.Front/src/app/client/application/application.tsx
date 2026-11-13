import './applications.scss';

import React, { useCallback, useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { useHistory, useParams } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

import { StoreType } from 'core/store';
import { DateTime } from 'utils/dateTime';
import { setAuthInfo } from 'data/auth/actions';
import { Flex, Button } from 'shared';

export const Application: React.FC = () => {
  const { t, i18n } = useTranslation();
  const application = useSelector((store: StoreType) => store.application.application);
  const history = useHistory();
  const dispatch = useDispatch();
  const { mode } = useParams<{ mode?: string }>();
  const isManual = mode?.toLocaleLowerCase() === 'manual';
  const logout = useCallback(() => {
    dispatch(setAuthInfo());
    history.push(`/login/` + (mode ?? ''));
  }, [dispatch, history, mode]);
  const returnToSurvey = useCallback(() => {
    history.push(`/` + (mode ?? ''));
  }, [history, mode]);
  const dayOfTheWeek = () => {
    const weekDays = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
    return weekDays[new Date().getDay()];
  };

  useEffect(() => {
    window.scrollTo(0, 0);
  }, []);

  if (application == null) return <div>{t('applicationPage.nodata')}</div>;
  const isEnglish = i18n.language == 'en' ? true : false;
  return (
    <div className="application">
      <div className="applicationResult">
        <div
          style={{
            textAlign: 'center',
            fontSize: '1rem',
            lineHeight: '2rem',
            padding: '1rem',
            background: '#fff'
          }}>
          <div className={dayOfTheWeek()} style={{ fontSize: '2rem', fontWeight: 'bold', lineHeight: '4rem' }}>
            {t('applicationPage.covidPass')}
          </div>
          <div
            style={{
              color: application.noSymptoms === true ? '#339966' : '#d90939',
              fontSize: '2rem',
              lineHeight: '3rem',
              margin: '0.5rem 0 0.5rem 0'
            }}>
            {application.noSymptoms === true ? 'CLEARED FOR WORK' : 'NOT CLEARED FOR WORK'}
          </div>
          {!isEnglish && (
            <>
              <div
                style={{
                  color: application.noSymptoms === true ? '#339966' : '#d90939',
                  fontSize: '1rem',
                  lineHeight: '3rem',
                  margin: '0.5rem 0 0.5rem 0'
                }}>
                {application.noSymptoms === true
                  ? t('applicationPage.noSymptoms_clearedForWork')
                  : t('applicationPage.symptoms_notClearedForWork')}
              </div>
            </>
          )}
          <div style={{ fontSize: '1rem', lineHeight: '1rem' }}>
            {application.noSymptoms === true ? (
              t('applicationPage.noSymptoms_description')
            ) : (
              <>
                {t('applicationPage.symptoms_description1')}
                <a href="tel:+16177248100">617-724-8100</a>. {t('applicationPage.symptoms_description2')}
                <a href="tel:+16037402800">603-740-2800</a>
              </>
            )}
          </div>
          <div style={{ borderBottom: '2px solid #ccc', fontSize: '3rem', marginTop: '1rem', padding: '0.5rem' }}>
            {DateTime.format(application.created, 'shortDay').toUpperCase()}
          </div>
          <div style={{ borderBottom: '2px solid #ccc', fontSize: '1.25rem', padding: '0.5rem' }}>
            {DateTime.format(application.created, 'datetime')}
          </div>
          <div
            style={{
              borderBottom: '2px solid #ccc',
              fontSize: '3rem',
              lineHeight: '3rem',
              padding: '0.5rem',
              wordWrap: 'break-word'
            }}>
            {application.firstName} {application.lastName}
          </div>
        </div>
        <div className="footerPanel">
          <div>
            {t('applicationPage.footer_sentToEmail')} - {application.email}
          </div>
          <div>{t('applicationPage.footer_healthConcerns')}</div>
          <div>
            {t('applicationPage.footer_needAssistance')}
            <a href="tel:+16177248100">617-724-8100</a>.
          </div>
          <Flex className="buttonsPanel" justifyContent="center">
            <Button view="link" onClick={logout}>
              {t('common.logout')}
            </Button>
            {isManual && <Button onClick={returnToSurvey}>{t('applicationPage.footer_returnToForm')}</Button>}
          </Flex>
        </div>
        <div>
          <footer>
            <p className="footerStyle">&#169; Copyright Partners HealthCare Innovation and Technology Department.</p>
          </footer>
        </div>
      </div>
    </div>
  );
};
