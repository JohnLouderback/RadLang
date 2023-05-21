import { installEmscriptenTask } from '../emscripten/tasks/install.js';

export const initRepoTask = {
  name: 'Initialize Repository',
  subTasks: [installEmscriptenTask]
};
