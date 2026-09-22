<script setup lang="ts">
import { ref } from 'vue'
const userName = ref('xxx')

userName.value = "test"

</script>

<template>
  <div class="flex justify-center w-full py-10">
    <UPageSection :title="`Welcome User : ${userName}`" :ui="{
      container: 'flex flex-col items-center justify-center text-center',
      links: 'justify-center'
    }" />
  </div>
</template>

<style></style>
