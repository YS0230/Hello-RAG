<script setup lang="ts">
import { reactive, watch } from 'vue'
import type { SystemRule, SystemRulePayload } from '@/api/systemRule'

const props = defineProps<{
  modelValue: boolean
  rule?: SystemRule | null
}>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  saved: [payload: SystemRulePayload]
}>()

const form = reactive<SystemRulePayload>({
  systemName: '',
  serialNumber: '',
  system: '',
  ruleDescription: '',
  recorder: '',
})

watch(
  () => props.modelValue,
  open => {
    if (open) {
      form.systemName = props.rule?.systemName ?? ''
      form.serialNumber = props.rule?.serialNumber ?? ''
      form.system = props.rule?.system ?? ''
      form.ruleDescription = props.rule?.ruleDescription ?? ''
      form.recorder = props.rule?.recorder ?? ''
    }
  },
)

function close() {
  emit('update:modelValue', false)
}

function submit() {
  emit('saved', { ...form })
  close()
}
</script>

<template>
  <Teleport to="body">
    <div
      v-if="modelValue"
      class="fixed inset-0 z-50 flex items-center justify-center backdrop-blur-sm"
      style="background: rgba(45, 58, 49, 0.35);"
    >
      <div
        class="w-full max-w-lg bg-background rounded-3xl p-8"
        style="box-shadow: 0 25px 50px -12px rgba(45, 58, 49, 0.15);"
      >
        <h2
          class="text-xl font-semibold text-heading mb-6"
          style="font-family: 'Playfair Display', Georgia, serif;"
        >
          {{ rule ? '編輯規則' : '新增規則' }}
        </h2>

        <form class="flex flex-col gap-4" @submit.prevent="submit">
          <div class="grid grid-cols-2 gap-4">
            <div class="flex flex-col gap-1.5">
              <label class="text-[0.8rem] text-text-muted font-medium tracking-wide">系統名稱</label>
              <input
                v-model="form.systemName"
                required
                class="px-4 py-2.5 rounded-full border border-border bg-background-soft text-text text-sm
                       focus:outline-none focus:ring-2 focus:ring-sage/40 focus:border-sage transition-all duration-300"
              />
            </div>
            <div class="flex flex-col gap-1.5">
              <label class="text-[0.8rem] text-text-muted font-medium tracking-wide">序號</label>
              <input
                v-model="form.serialNumber"
                required
                class="px-4 py-2.5 rounded-full border border-border bg-background-soft text-text text-sm
                       focus:outline-none focus:ring-2 focus:ring-sage/40 focus:border-sage transition-all duration-300"
              />
            </div>
          </div>

          <div class="flex flex-col gap-1.5">
            <label class="text-[0.8rem] text-text-muted font-medium tracking-wide">系統</label>
            <input
              v-model="form.system"
              required
              class="px-4 py-2.5 rounded-full border border-border bg-background-soft text-text text-sm
                     focus:outline-none focus:ring-2 focus:ring-sage/40 focus:border-sage transition-all duration-300"
            />
          </div>

          <div class="flex flex-col gap-1.5">
            <label class="text-[0.8rem] text-text-muted font-medium tracking-wide">規則說明</label>
            <textarea
              v-model="form.ruleDescription"
              required
              rows="4"
              class="px-4 py-2.5 rounded-2xl border border-border bg-background-soft text-text text-sm
                     focus:outline-none focus:ring-2 focus:ring-sage/40 focus:border-sage transition-all duration-300 resize-none"
            />
          </div>

          <div class="flex flex-col gap-1.5">
            <label class="text-[0.8rem] text-text-muted font-medium tracking-wide">填寫人</label>
            <input
              v-model="form.recorder"
              required
              class="px-4 py-2.5 rounded-full border border-border bg-background-soft text-text text-sm
                     focus:outline-none focus:ring-2 focus:ring-sage/40 focus:border-sage transition-all duration-300"
            />
          </div>

          <div class="flex justify-end gap-3 pt-2">
            <button
              type="button"
              class="px-6 py-2.5 rounded-full text-sm text-text-muted border border-border
                     hover:bg-background-soft transition-all duration-300"
              @click="close"
            >
              取消
            </button>
            <button
              type="submit"
              class="px-6 py-2.5 rounded-full text-sm bg-accent text-white font-medium
                     hover:opacity-85 transition-all duration-300"
            >
              儲存
            </button>
          </div>
        </form>
      </div>
    </div>
  </Teleport>
</template>
