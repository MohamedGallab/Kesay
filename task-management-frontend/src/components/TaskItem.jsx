import React, { useState } from 'react';
import { deleteTask } from '../services/api';
import TaskForm from './TaskForm';
import { toast } from 'react-toastify';

const TaskItem = ({ task, removeTask }) => {
    const [editing, setEditing] = useState(false);

    const handleDelete = async () => {
        try {
            await deleteTask(task.id);
            removeTask(task.id);
            toast.success('Task deleted successfully');
        } catch (error) {
            toast.error('An error occurred while deleting the task');
            console.error('TaskItem Delete Error:', error);
        }
    };

    return (
        <div>
            {editing ? (
                <TaskForm task={task} setEditing={setEditing} />
            ) : (
                <div>
                    <h3>{task.title}</h3>
                    <p>{task.description}</p>
                    <button onClick={() => setEditing(true)}>Edit</button>
                    <button onClick={handleDelete}>Delete</button>
                </div>
            )}
        </div>
    );
};

export default TaskItem;
