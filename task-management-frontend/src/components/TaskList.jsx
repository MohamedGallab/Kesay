// src/components/TaskList.jsx
import React, { useEffect, useState } from 'react';
import { getTasks } from '../services/api';
import TaskItem from './TaskItem';
import TaskForm from './TaskForm';

const TaskList = () => {
    const [tasks, setTasks] = useState([]);

    useEffect(() => {
        async function fetchData() {
            const result = await getTasks();
            setTasks(result.data);
        }
        fetchData();
    }, []);

    const removeTask = (taskId) => {
        setTasks(tasks.filter(task => task.id !== taskId));
    };

    const addNewTask = (task) => {
        setTasks([...tasks, task]);
    };

    return (
        <div>
            <TaskForm addNewTask={addNewTask} />
            {tasks.map(task => (
                <TaskItem key={task.id} task={task} removeTask={removeTask} />
            ))}
        </div>
    );
};

export default TaskList;
