<script setup lang="ts">
import * as z from 'zod'
import type { FormSubmitEvent } from '@nuxt/ui'
import { reactive } from 'vue'
import { useRouter } from 'vue-router'

// @ts-ignore
import { authenUser } from '@/api/backendService'

const router = useRouter()

const schema = z.object({
  user: z
    .string('Please fill User')
    .min(6, 'Must be 6-20 characters long')
    .max(20, 'Must be 6-20 characters long')
    .regex(/^\S+$/, { message: "User cannot contain spaces" }),
  password: z
    .string('Please fill password')
    .min(8, 'Must be 8-20 characters long')
    .max(20, 'Must be 8-20 characters long')
    .regex(/^\S+$/, { message: "Password cannot contain spaces" }),
})

type Schema = z.output<typeof schema>

const state = reactive<Partial<Schema>>({
  user: '',
  password: ''
})

async function onSubmit(event: FormSubmitEvent<Schema>) {
  // console.log(event.data)

  try {
    const response = await authenUser({
      username: event.data.user,
      password: event.data.password
    })
    if (response.code === 200) {
      sessionStorage.setItem('token', response.message)

      //test show modal success
      alert('success')

      router.push('/member')
    } else {
      alert(response.message)
    } 
  } catch (error) {
    alert('error')
    console.log(error)
  }
}
</script>

<template>
  <div class="flex justify-center w-full py-10">
    <UContainer>
      <UPageHeader title="Login" class="space-y-4"
        :ui="{ root: 'border-none', container: 'flex flex-col items-center justify-center text-center w-full', title: 'text-center text-primary' }" />
      <UForm :schema="schema" :state="state" class="mx-auto w-64 max-w-md space-y-4" @submit="onSubmit">
        <UFormField label="User" name="user">
          <UInput class="w-full" v-model="state.user" maxlength="20" />
        </UFormField>

        <UFormField label="Password" name="password">
          <UInput class="w-full" v-model="state.password" type="password" maxlength="20" />
        </UFormField>

        <UButton size="xl" class="w-full justify-center" type="submit">
          Login
        </UButton>

        <ULink class="flex w-full justify-center text-center my-2" to="/register">Register a new member ?</ULink>
      </UForm>
    </UContainer>
  </div>
</template>
