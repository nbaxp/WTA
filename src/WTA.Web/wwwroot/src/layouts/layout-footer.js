import html from '../utils/index.js';
import { useAppStore } from '../store/index.js';

export default {
  template: html` <div class="flex justify-center items-center w-full h-full">
    {{ $t('copyright') }} v {{ appStore.version }}
  </div>`,
  setup() {
    return {
      appStore: useAppStore(),
    };
  },
};
