import {NotificationManager} from 'react-notifications';

export function createNotification(type, message, title = undefined, delay = undefined)
{
	// info | success | warning | error
	if ( typeof NotificationManager[type] == "function" )
	{
		NotificationManager[type](message, title, delay);
	}

	// switch (type) 
	// {
	// 	case 'info':
	// 		NotificationManager.info(message);
	// 		break;
	// 	case 'success':
	// 		NotificationManager.success(message, title);
	// 		break;
	// 	case 'warning':
	// 		NotificationManager.warning(message, title, delay);
	// 		break;
	// 	case 'error':
	// 		NotificationManager.error('Error message', 'Click me!', 5000, () => { });
	// 		break;
	// }
}
