<script setup lang="ts">
defineProps<{ message: string }>()
</script>

<template>
  <div class="error">{{ message }}</div>
</template>

<style scoped>
.error { color: #c00; padding: 0.5rem; border: 1px solid #c00; background: #fee; }
</style>
