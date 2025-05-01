<template>
  <v-container fluid class="pa-4 d-flex justify-center">
    <v-row class="max-width-container">
      <v-col cols="12">
        <v-card class="mx-auto">
          <v-card-title class="py-4 d-flex align-center px-4">
            <v-btn-toggle v-model="selectedType" mandatory>
              <v-btn value="PERSONAL">Personal</v-btn>
              <v-btn value="WORK">Work</v-btn>
            </v-btn-toggle>
            <v-spacer></v-spacer>
            <v-btn @click="logout" icon="mdi-logout" variant="text" title="Logout"></v-btn>
          </v-card-title>

          <v-card-text class="px-4">
            <div class="d-flex align-center mb-4">
              <v-text-field
                v-model="searchQuery"
                label="Search todos..."
                prepend-inner-icon="mdi-magnify"
                clearable
                variant="outlined"
                density="comfortable"
                hide-details
                class="flex-grow-1"
              ></v-text-field>
              <v-btn
                color="primary"
                @click="showAddForm = !showAddForm"
                size="small"
                :title="showAddForm ? 'Hide form' : 'Add a new todo'"
                class="ml-4"
                icon
              >
                <v-icon :class="{ 'rotate-45': showAddForm }">mdi-plus</v-icon>
              </v-btn>
            </div>

            <v-expand-transition>
              <v-form @submit.prevent="addTodo" class="mb-6" v-if="showAddForm">
                <v-card variant="flat">
                  <v-card-title class="px-0 pt-0">Add New Todo</v-card-title>
                  <v-text-field
                    v-model="newTodo.title"
                    label="Title *"
                    required
                    variant="outlined"
                    density="comfortable"
                    class="mb-2"
                  ></v-text-field>
                  <v-textarea
                    v-model="newTodo.description"
                    label="Description (optional)"
                    variant="outlined"
                    density="comfortable"
                    rows="2"
                    class="mb-2"
                  ></v-textarea>
                  <div class="d-flex justify-end">
                    <v-btn
                      color="grey-lighten-1"
                      class="mr-2"
                      @click="cancelAddTodo"
                    >
                      Cancel
                    </v-btn>
                    <v-btn
                      color="primary"
                      type="submit"
                      :disabled="!newTodo.title"
                    >
                      Add Todo
                    </v-btn>
                  </div>
                </v-card>
              </v-form>
            </v-expand-transition>

            <v-list class="bg-transparent">
              <draggable 
                v-model="filteredTodos" 
                item-key="id"
                :disabled="!!searchQuery"
                @end="onDragEnd"
                ghost-class="sortable-ghost"
                chosen-class="sortable-chosen"
                drag-class="sortable-drag"
                :animation="200"
                :delay="50"
                :delayOnTouchOnly="true"
              >
                <template #item="{element: todo}">
                  <TodoItem 
                    :todo="todo"
                    :isDraggable="!searchQuery"
                    @toggle="toggleTodo"
                    @delete="deleteTodo"
                    @edit="handleEdit"
                  />
                </template>
              </draggable>
              
              <v-alert 
                v-if="todos.length === 0" 
                type="info" 
                class="mt-4"
                color="primary"
                variant="tonal"
              >
                No todos found. Create one to get started!
              </v-alert>
            </v-list>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup>
import { ref, computed, watch, onMounted, reactive } from 'vue'
import axios from 'axios'
import { debounce } from 'lodash'
import { useRouter } from 'vue-router'
import draggable from 'vuedraggable'
import TodoItem from './TodoItem.vue'

const router = useRouter()
const API_URL = 'http://localhost:5213/api/todo'
const todos = ref([])
const searchQuery = ref('')
const selectedType = ref('PERSONAL')
const showAddForm = ref(false)
const editingTodos = reactive({})

const personalTodo = ref({
  title: '',
  description: '',
  isCompleted: false
})

const workTodo = ref({
  title: '',
  description: '',
  isCompleted: false
})

const newTodo = computed({
  get: () => selectedType.value === 'PERSONAL' ? personalTodo.value : workTodo.value,
  set: (value) => {
    if (selectedType.value === 'PERSONAL') {
      personalTodo.value = value
    } else {
      workTodo.value = value
    }
  }
})

onMounted(() => {
  const user = JSON.parse(localStorage.getItem('user'))
  if (!user || !user.token) {
    router.push('/login')
    return
  }
  
  axios.defaults.headers.common['Authorization'] = `Bearer ${user.token}`
  
  fetchTodos()
})

const logout = () => {
  localStorage.removeItem('user')
  delete axios.defaults.headers.common['Authorization']
  router.push('/login')
}

const fetchTodos = async () => {
  try {
    const response = await axios.get(`${API_URL}?type=${selectedType.value}`)
    todos.value = response.data
  } catch (error) {
    console.error('Error fetching todos:', error)
    if (error.response?.status === 401) {
      router.push('/login')
    }
  }
}

const addTodo = async () => {
  try {
    const todoToAdd = {
      title: newTodo.value.title,
      description: newTodo.value.description || '',
      isCompleted: false,
      type: selectedType.value
    }
    const response = await axios.post(API_URL, todoToAdd)
    todos.value.push(response.data)
    
    if (selectedType.value === 'PERSONAL') {
      personalTodo.value.title = ''
      personalTodo.value.description = ''
    } else {
      workTodo.value.title = ''
      workTodo.value.description = ''
    }
    
    showAddForm.value = false
  } catch (error) {
    console.error('Error adding todo:', error)
    if (error.response?.status === 401) {
      router.push('/login')
    }
  }
}

const cancelAddTodo = () => {
  if (selectedType.value === 'PERSONAL') {
    personalTodo.value.title = ''
    personalTodo.value.description = ''
  } else {
    workTodo.value.title = ''
    workTodo.value.description = ''
  }
  showAddForm.value = false
}

const toggleTodo = async (todo) => {
  try {
    const index = todos.value.findIndex(t => t.id === todo.id);
    if (index !== -1) {
      todos.value[index].isCompleted = todo.isCompleted;
    }
    
    const todoToUpdate = {
      ...todo,
      type: selectedType.value
    }
    await axios.put(`${API_URL}/${todo.id}`, todoToUpdate)
  } catch (error) {
    const index = todos.value.findIndex(t => t.id === todo.id);
    if (index !== -1) {
      todos.value[index].isCompleted = !todo.isCompleted;
    }
    
    console.error('Error updating todo:', error)
    if (error.response?.status === 401) {
      router.push('/login')
    }
  }
}

const deleteTodo = async (id) => {
  try {
    await axios.delete(`${API_URL}/${id}?type=${selectedType.value}`)
    todos.value = todos.value.filter(todo => todo.id !== id)
  } catch (error) {
    console.error('Error deleting todo:', error)
    if (error.response?.status === 401) {
      router.push('/login')
    }
  }
}

const handleEdit = async (updatedTodo) => {
  try {
    const todoToUpdate = {
      ...updatedTodo,
      type: selectedType.value
    }
    
    await axios.put(`${API_URL}/${updatedTodo.id}`, todoToUpdate)
    
    const index = todos.value.findIndex(t => t.id === updatedTodo.id)
    if (index !== -1) {
      todos.value[index] = {
        ...todos.value[index],
        title: updatedTodo.title,
        description: updatedTodo.description
      }
    }
    
    delete editingTodos[updatedTodo.id]
  } catch (error) {
    console.error('Error updating todo:', error)
    if (error.response?.status === 401) {
      router.push('/login')
    }
  }
}

const debouncedSearch = debounce(async () => {
  if (!searchQuery.value) {
    await fetchTodos()
    return
  }
  try {
    const response = await axios.get(`${API_URL}/search?term=${searchQuery.value}&type=${selectedType.value}`)
    todos.value = response.data
  } catch (error) {
    console.error('Error searching todos:', error)
    if (error.response?.status === 401) {
      router.push('/login')
    }
  }
}, 300)

watch(searchQuery, () => {
  debouncedSearch()
})

watch(selectedType, () => {
  fetchTodos()
})

const filteredTodos = computed({
  get: () => todos.value,
  set: (value) => {
    todos.value = value;
  }
});

const onDragEnd = async () => {
  if (searchQuery.value) return;
  
  try {
    const updatePayload = todos.value.map((todo, index) => ({
      id: todo.id,
      order: index,
      type: selectedType.value
    })); 
    
    const response = await axios.post(`${API_URL}/reorder`, updatePayload);
    
    await fetchTodos();
    
  } catch (error) {
    console.error('Error updating todo order:', error);
    await fetchTodos();
  }
}
</script>

<style scoped>
.v-list-item {
  border: 1px solid rgba(0, 0, 0, 0.12);
  transition: all 0.3s ease;
}

.v-list-item:hover {
  background-color: rgba(0, 0, 0, 0.02);
}

.editable-todo {
  padding-top: 16px !important;
  padding-bottom: 16px !important;
}

.editable-todo .v-btn {
  margin-top: 8px;
}

.editable-todo .w-100 {
  padding-top: 6px;
}

.draggable-item {
  cursor: grab;
  position: relative;
}

.draggable-item:hover {
  background-color: rgba(0, 0, 0, 0.04) !important;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.draggable-item:hover::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0;
  width: 4px;
  height: 100%;
  background-color: var(--v-theme-primary, #e57373);
  border-top-left-radius: 4px;
  border-bottom-left-radius: 4px;
  opacity: 0.7;
}

.draggable-item:active {
  cursor: grabbing;
  background-color: rgba(0, 0, 0, 0.08) !important;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.sortable-ghost {
  opacity: 0;
}

.sortable-chosen {
  opacity: 1;
}

.sortable-drag {
  opacity: 1;
}

.rotate-45 {
  transform: rotate(45deg);
}

.max-width-container {
  width: 100%;
  max-width: 1000px;
  min-width: min(800px, 98vw);
  margin: 0 auto;
}

@media (max-width: 900px) {
  .max-width-container {
    min-width: 80vw;
  }
}

.add-todo-btn {
  height: 40px;
  width: 40px;
  min-width: 40px;
}

.checkbox-placeholder {
  width: 40px;
  height: 40px;
}

:deep(.v-btn:focus),
:deep(.v-btn:active) {
  outline: none !important;
  box-shadow: none !important;
}

:deep(.v-btn-toggle .v-btn:focus),
:deep(.v-btn-toggle .v-btn:active) {
  outline: none !important;
  box-shadow: none !important;
}

@media (max-width: 600px) {
  .v-list-item {
    padding: 8px !important;
  }
  
  .v-list-item-title {
    font-size: 0.9rem !important;
    line-height: 1.2 !important;
  }
  
  .v-list-item-subtitle {
    font-size: 0.75rem !important;
  }
  
  .edit-btn, .delete-btn, .cancel-btn, .save-btn {
    min-width: 0 !important;
    padding: 0 4px !important;
  }
  
  .editable-todo {
    padding-top: 16px !important;
    padding-bottom: 12px !important;
  }
  
  .editable-todo .w-100 {
    padding: 0 !important;
    width: 100% !important;
  }
  
  .edit-container {
    width: 100% !important;
    padding-right: 8px !important;
    padding-left: 0 !important;
  }
  
  .editable-todo .v-field {
    padding-bottom: 4px !important;
  }
  
  .editable-todo .mb-3 {
    margin-bottom: 8px !important;
  }
  
  :deep(.v-card-title) {
    padding: 12px 16px !important;
  }
  
  :deep(.v-btn-toggle .v-btn) {
    padding: 0 8px !important;
    font-size: 0.85rem !important;
  }
}

@media (max-width: 375px) {
  .cancel-btn, .save-btn {
    margin: 0 4px !important;
    padding: 0 8px !important;
    font-size: 0.8rem !important;
  }
}
</style> 