import type {PageServerLoad} from './$types';

export const load = ( async ({fetch}) => {
    const response = await fetch('http://localhost:5001/')
    return await response.json();
}) satisfies PageServerLoad
