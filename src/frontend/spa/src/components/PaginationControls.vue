<script setup lang="ts">
defineProps<{
  currentPage: number
  totalPages: number
}>()

const emit = defineEmits<{ (e: 'change', page: number): void }>()
</script>

<template>
  <div class="pagination">
    <button :disabled="currentPage <= 1" @click="emit('change', currentPage - 1)">Prev</button>
    <span>{{ currentPage }} / {{ totalPages }}</span>
    <button :disabled="currentPage >= totalPages" @click="emit('change', currentPage + 1)">Next</button>
  </div>
</template>
