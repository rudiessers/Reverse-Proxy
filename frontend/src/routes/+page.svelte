<script lang="ts">
	import type { PageData } from './$types';
	export let data: PageData;


	let deelnames: string;
	let aanvragen: string;

	async function getAanvragen() {
		let response = await fetch('http://localhost:5001/aanvragen');
		aanvragen = await response.json();
	}

	async function getDeelnames() {
		let response = await fetch('http://localhost:5301/');
		deelnames = await response.json();

	}
</script>

<h1>Welcome to the Demo</h1>

{#if data.msg}
	<p>{data.msg}</p>
{/if}


<button on:click={getDeelnames}>Get Deelnames</button>
{#if deelnames}
	<p>{deelnames.msg}</p>
{/if}

<button on:click={getAanvragen}>Get Aanvragen</button>
{#if aanvragen}
	<p>{aanvragen.msg}</p>
{/if}