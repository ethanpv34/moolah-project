<template>
  <v-list-item
    :key="todo.id"
    :class="{ 
      'text-decoration-line-through': todo.isCompleted && !isEditing,
      'editable-todo': isEditing,
      'draggable-item': isDraggable
    }"
    class="mb-2 rounded-lg"
    variant="outlined"
  >
    <template v-slot:prepend>
      <v-checkbox
        v-if="!isEditing"
        v-model="isCompleted"
        color="primary"
        hide-details
      ></v-checkbox>
      <div v-else class="checkbox-placeholder d-none d-sm-block"></div>
    </template>

    <div v-if="isEditing" class="w-100 text-left px-2 edit-container">
      <v-text-field
        v-model="editedTitle"
        label="Title"
        variant="outlined"
        density="compact"
        class="mb-3 mt-2"
        hide-details
      ></v-text-field>
      <v-textarea
        v-model="editedDescription"
        label="Description"
        variant="outlined"
        density="compact"
        rows="2"
        hide-details
        class="mb-0 mb-sm-0"
      ></v-textarea>
      
      <div class="d-flex justify-center mt-2 d-sm-none">
        <v-btn
          variant="text"
          color="grey-darken-1"
          @click="cancelEdit"
          class="mr-2 cancel-btn"
          density="comfortable"
          size="small"
        >
          CANCEL
        </v-btn>
        <v-btn
          variant="text"
          color="success"
          @click="saveEdit"
          density="comfortable"
          size="small"
          class="save-btn"
        >
          SAVE
        </v-btn>
      </div>
    </div>

    <div v-else class="text-left">
      <v-list-item-title class="text-body-1">{{ todo.title }}</v-list-item-title>
      <v-list-item-subtitle 
        v-if="todo.description" 
        class="text-caption text-grey-darken-1 mt-1"
      >
        {{ truncateDescription(todo.description, 100) }}
      </v-list-item-subtitle>
    </div>

    <template v-slot:append>
      <div v-if="isEditing" class="d-none d-sm-block">
        <v-btn
          variant="text"
          color="grey-darken-1"
          @click="cancelEdit"
          class="mr-1 cancel-btn"
          density="comfortable"
          size="small"
        >
          <span>CANCEL</span>
        </v-btn>
        <v-btn
          variant="text"
          color="success"
          @click="saveEdit"
          density="comfortable"
          size="small"
          class="save-btn"
        >
          <span>SAVE</span>
        </v-btn>
      </div>
      <div v-else>
        <v-btn
          variant="text"
          color="grey-darken-1"
          @click="startEdit"
          class="mr-1 edit-btn"
          density="comfortable"
          size="small"
        >
          <span class="d-none d-sm-inline">EDIT</span>
          <v-icon class="d-sm-none">mdi-pencil</v-icon>
        </v-btn>
        <v-btn
          variant="text"
          color="primary"
          @click="$emit('delete', todo.id)"
          density="comfortable"
          size="small"
          class="delete-btn"
        >
          <span class="d-none d-sm-inline">DELETE</span>
          <v-icon class="d-sm-none">mdi-delete</v-icon>
        </v-btn>
      </div>
    </template>
  </v-list-item>
</template>

<script setup>
import { ref, computed } from 'vue';

const props = defineProps({
  todo: {
    type: Object,
    required: true
  },
  isDraggable: {
    type: Boolean,
    default: true
  }
});

const emit = defineEmits(['toggle', 'delete', 'edit', 'cancel-edit']);

const isEditing = ref(false);
const editedTitle = ref('');
const editedDescription = ref('');

const isCompleted = computed({
  get: () => props.todo.isCompleted,
  set: (value) => {
    const updatedTodo = { ...props.todo, isCompleted: value };
    emit('toggle', updatedTodo);
  }
});

const startEdit = () => {
  editedTitle.value = props.todo.title;
  editedDescription.value = props.todo.description || '';
  isEditing.value = true;
};

const saveEdit = () => {
  const updatedTodo = {
    ...props.todo,
    title: editedTitle.value,
    description: editedDescription.value
  };
  
  emit('edit', updatedTodo);
  isEditing.value = false;
};

const cancelEdit = () => {
  isEditing.value = false;
  emit('cancel-edit');
};

const truncateDescription = (text, maxLength) => {
  if (!text || text.length <= maxLength) return text;
  return text.substring(0, maxLength) + '...';
};
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

.checkbox-placeholder {
  width: 40px;
  height: 40px;
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
}

@media (max-width: 375px) {
  .cancel-btn, .save-btn {
    margin: 0 4px !important;
    padding: 0 8px !important;
    font-size: 0.8rem !important;
  }
}
</style> 