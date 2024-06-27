// src/components/TaskForm.jsx

import React, { useState } from 'react';
import { addTask, updateTask } from '../services/api';
import { toast } from 'react-toastify';

const TaskForm = ({ task, setEditing, addNewTask }) => {
    const [title, setTitle] = useState(task ? task.title : '');
    const [description, setDescription] = useState(task ? task.description : '');

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            if (task) {
                await updateTask(task.id, { title, description });
                setEditing(false);
                toast.success('Task updated successfully');
            } else {
                const newTask = await addTask({ title, description });
                addNewTask(newTask.data);
                setTitle('');
                setDescription('');
                toast.success('Task added successfully');
            }
        } catch (error) {
            toast.error('An error occurred');
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <input
                type="text"
                placeholder="Title"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
            />
            <textarea
                placeholder="Description"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
            />
            <button type="submit">Save</button>
        </form>
    );
};

export default TaskForm;
