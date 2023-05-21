import React, { FC, useState } from 'react';

import { observer } from 'mobx-react-lite';

import { Installer } from '../../utils/Task/Installer.js';
import { installEmscriptenTask } from './tasks/install.js';

export const EmscriptenInstallUI: FC = observer(() => {
  const [task] = useState(installEmscriptenTask);

  return <Installer task={task} />;
});
