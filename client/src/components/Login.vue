<template>
  <v-container class="fill-height" fluid>
    <v-row class="align-center justify-center">
      <v-col cols="12" sm="8" md="6" lg="4" xl="3" class="login-container">
        <v-card class="elevation-12">
          <v-tabs v-model="tab" grow>
            <v-tab value="login">Login</v-tab>
            <v-tab value="register">Register</v-tab>
          </v-tabs>

          <v-card-text class="py-6">
            <v-alert v-if="error" type="error" class="mb-4">
              {{ error }}
            </v-alert>
            
            <v-window v-model="tab">
              <v-window-item value="login">
                <v-form @submit.prevent="login" class="pt-4">
                  <v-text-field
                    v-model="loginForm.username"
                    label="Username"
                    prepend-inner-icon="mdi-account"
                    variant="outlined"
                    required
                    class="input-field-padding"
                  ></v-text-field>

                  <v-text-field
                    v-model="loginForm.password"
                    label="Password"
                    prepend-inner-icon="mdi-lock"
                    type="password"
                    variant="outlined"
                    required
                  ></v-text-field>
                  
                  <v-btn
                    type="submit"
                    color="primary"
                    block
                    class="mt-4"
                    :loading="loading"
                  >
                    Login
                  </v-btn>
                </v-form>
              </v-window-item>

              <v-window-item value="register">
                <v-form @submit.prevent="register" class="pt-4">
                  <v-text-field
                    v-model="registerForm.username"
                    label="Username"
                    prepend-inner-icon="mdi-account"
                    variant="outlined"
                    required
                    class="input-field-padding"
                  ></v-text-field>

                  <v-text-field
                    v-model="registerForm.password"
                    label="Password"
                    prepend-inner-icon="mdi-lock"
                    type="password"
                    variant="outlined"
                    required
                  ></v-text-field>
                  
                  <v-btn
                    type="submit"
                    color="primary"
                    block
                    class="mt-4"
                    :loading="loading"
                  >
                    Register
                  </v-btn>
                </v-form>
              </v-window-item>
            </v-window>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup>
import { ref } from 'vue';
import axios from 'axios';
import { useRouter } from 'vue-router';

const router = useRouter();
const tab = ref('login');
const loading = ref(false);
const error = ref('');

const loginForm = ref({
  username: '',
  password: ''
});

const registerForm = ref({
  username: '',
  password: ''
});

const API_URL = 'http://localhost:5213/api/auth';

const login = async () => {
  if (!loginForm.value.username || !loginForm.value.password) {
    error.value = 'Please fill in all fields';
    return;
  }
  
  loading.value = true;
  error.value = '';
  
  try {
    const response = await axios.post(`${API_URL}/login`, loginForm.value);
    
    localStorage.setItem('user', JSON.stringify(response.data));
    
    axios.defaults.headers.common['Authorization'] = `Bearer ${response.data.token}`;
    
    router.push('/todos');
  } catch (err) {
    console.error('Login error:', err);
    error.value = err.response?.data || 'Login failed. Please try again.';
  } finally {
    loading.value = false;
  }
};

const register = async () => {
  if (!registerForm.value.username || !registerForm.value.password) {
    error.value = 'Please fill in all fields';
    return;
  }
  
  loading.value = true;
  error.value = '';
  
  try {
    const response = await axios.post(`${API_URL}/register`, registerForm.value);
    
    localStorage.setItem('user', JSON.stringify(response.data));
    
    axios.defaults.headers.common['Authorization'] = `Bearer ${response.data.token}`;
    
    router.push('/todos');
  } catch (err) {
    console.error('Registration error:', err);
    error.value = err.response?.data || 'Registration failed. Please try again.';
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.login-container {
  min-width: min(400px, 95vw);
  max-width: 500px;
}

.input-field-padding {
  padding-top: 12px;
}

@media (max-width: 600px) {
  .login-container {
    min-width: 80vw;
  }
}
</style> 