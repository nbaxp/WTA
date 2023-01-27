import html from 'html';
import request from '../request/index.js';
import { cloneDeep } from '../utils/index.js';

const template = html`<el-form
    ref="formRef"
    v-loading="loading"
    element-loading-background="rgba(0, 0, 0, 0)"
    :inline="model.inline"
    :model="model.data"
    :label-width="model.labelWidth ?? 'auto'"
    :label-suffix="model.labelSufix ?? '：'"
    :label-position="model.labelPosition"
    :class="model.mode"
  >
    <slot name="header">
      <h2
        v-if="schema?.title"
        class="text-center"
      >
        {{ schema?.title }}
      </h2>
    </slot>
    <app-form-item
      v-model="model.data"
      :schema="schema"
      :validate="!model.disableValidation"
      :mode="model.mode"
      :errors="errors"
    />
    <slot name="footer">
      <el-form-item>
        <el-button
          :disabled="disabled"
          type="primary"
          @click="submit"
        >
          提交
        </el-button>
        <el-button @click="reset">重置</el-button>
      </el-form-item>
    </slot>
  </el-form>`;

export default {
  template,
  components: { VCharts },
  props: {
    options: {
      default: {},
    },
    autoresize: {
      default: true,
    },
    width: {
      default: '100%',
    },
    height: {
      default: '100%',
    },
  },
  setup() {
    const renderChart = ref(false);
    nextTick(() => {
      renderChart.value = true;
    });
    return {
      renderChart,
    };
  },
};
