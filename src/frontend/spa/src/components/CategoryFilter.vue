<script setup lang="ts">
import { ref, watch } from 'vue'

const props = defineProps<{
  categories: { id: number; name: string }[]
  modelValue?: number | null
}>()

const emit = defineEmits<{ (e: 'update:modelValue', v: number | null): void }>()

const selected = ref<number | null>(props.modelValue ?? null)
watch(selected, v => emit('update:modelValue', v))
</script>

<template>
  <select v-model="selected">
    <option :value="null">All Categories</option>
    <option v-for="c in categories" :key="c.id" :value="c.id">{{ c.name }}</option>
  </select>
</template>
