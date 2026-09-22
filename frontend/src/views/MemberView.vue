<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
// @ts-ignore
import { getMember } from '@/api/backendService'

const router = useRouter()

const userName = ref('')

onMounted(async () => {
  if (!sessionStorage.getItem('token')) {
    alert('token expired or no token')
    router.push('/')
    return
  }

  try {
    // console.log(sessionStorage.getItem('token'))
    const response = await getMember()
    if (response.status == 200) {
      userName.value = response.data
    } else {
      router.push('/')
    }
  } catch (error) {
    alert('error')
    console.log(error)
  }

});

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
