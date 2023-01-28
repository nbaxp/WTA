import html from './utils/index.js';
import { ref } from 'vue';
// import zh from 'element-plus/locale/zh-cn.js';
// import en from 'element-plus/locale/en.js';
import { useAppStore } from './store/index.js';

const template = html`<el-config-provider
  :size="size"
  :button="{ autoInsertSpace: true }"
  :locale="currentLocale"
>
  <slot />
</el-config-provider>`;

export default {
  template,
  setup() {
    const currentLocale = ref();
    const appStore = useAppStore();
    return {
      currentLocale,
      appStore,
    };
  },
};
