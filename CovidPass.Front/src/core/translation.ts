// © 2020 Partners HealthCare Technology & Innovation Department
import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import Backend from 'i18next-xhr-backend';

import { appUrlPath } from 'utils/url';

export const translation = i18n
  .use(Backend) // load translation using xhr
  .use(initReactI18next) // passes i18n down to react-i18next
  .init({
    lng: localStorage.getItem('lang') ?? 'en',
    fallbackLng: 'en',
    backend: {
      loadPath: appUrlPath + '/locales/{{lng}}.json'
    },
    interpolation: {
      escapeValue: false
    },
    react: {
      useSuspense: false
    }
  });
