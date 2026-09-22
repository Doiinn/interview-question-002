<script setup lang="ts">
import { RouterView } from 'vue-router'
</script>

<template>
  <UHeader title="Example.com" to="/" :toggle="false">
    <template #right>
      The Home of Example System
    </template>
  </UHeader>

  <RouterView />
</template>
