<template>
	<div class="scroll-container">
		<div style="width: 100%;" class="scroll-slot-container">
			<slot></slot>
		</div>
	</div>
</template>

<script lang="ts">
	import Vue from 'vue';
	import SimpleBar from 'simplebar';
	export default Vue.extend({
		components: {
		},
		props: {
		},
		data() {
			return {
			};
		},
		mounted() {
			this.$nextTick(() => {
				var scrollContainer: HTMLElement = <HTMLElement>window.document.getElementsByClassName('scroll-slot-container')[0];
				var simplebar = new SimpleBar(scrollContainer, {
					autoHide: false,
					scrollbarMaxSize: 16,
					scrollbarMinSize: 16,
				});
				var scrollElement = simplebar.getScrollElement();
				scrollElement.addEventListener('scroll', (e) => {
					var scrollHeight = scrollElement.scrollHeight;
					var scrollTop = scrollElement.scrollTop;
					var offsetHeight = (<any>scrollElement).offsetHeight;
					if (scrollHeight * 0.85 < (scrollTop + offsetHeight)) {
						this.$emit('load-more');
						this.$nextTick(() => {
							simplebar.recalculate();
						});
					}
				});
			})
		},
		methods: {
			scrollDown(pixels) {
				//var scrollContainer = window.document.getElementsByClassName('scroll-slot-container')[0];
				//var containerHeight = (<any>scrollContainer).offsetHeight;
				//if (scrollContainer.scrollHeight - scrollContainer.scrollTop - containerHeight < pixels) {
				//	scrollContainer.scroll(0, scrollContainer.scrollHeight - containerHeight);
				//} else {
				//	scrollContainer.scroll(0, scrollContainer.scrollTop + pixels);
				//}
				////var event = document.createEvent("HTMLEvents");
				////event.initEvent("scroll", true, true);
				////scrollContainer.dispatchEvent(event);
				//this.updateSliderPosition();
			},
			scrollUp(pixels) {
				//var scrollContainer = window.document.getElementsByClassName('scroll-slot-container')[0];
				
				//if (scrollContainer.scrollTop < pixels) {
				//	scrollContainer.scroll(0, 0);
				//} else {
				//	scrollContainer.scroll(0, scrollContainer.scrollTop - pixels);
				//}
				////var event = document.createEvent("HTMLEvents");
				////event.initEvent("scroll", true, true);
				////scrollContainer.dispatchEvent(event);
				//this.updateSliderPosition();
			},
			scrollDownStart(pixelsPerSecond) {
				//// update 10 times a second
				//const pixelsPerFrame = pixelsPerSecond / 20;
				//var interval = window.setInterval(() => {
				//	this.scrollDown(pixelsPerFrame);
				//}, 50);
				//window.addEventListener('mouseup', () => {
				//	window.clearInterval(interval);
				//});
			},
			scrollUpStart(pixelsPerSecond) {
				//const pixelsPerFrame = pixelsPerSecond / 20;
				//var interval = window.setInterval(() => {
				//	this.scrollUp(pixelsPerFrame);
				//}, 50);
				//window.addEventListener('mouseup', () => {
				//	window.clearInterval(interval);
				//});
			},
			sliderMousedown() {

			},
			updateSliderPosition() {
				// Find the percentage we are scrolled to, and then move 
				//// slider to fit.
				//var scrollContainer = window.document.getElementsByClassName('scroll-slot-container')[0];
				//var totalHeight = scrollContainer.scrollHeight;
				//var containerHeight = (<any>scrollContainer).offsetHeight;
				//// scrolls from top == 0 to top == scrollheight-offsetheight
				//var ratio = scrollContainer.scrollTop / (totalHeight - containerHeight);
				//var remaining = (containerHeight - (2 * this.sliderTopOffset)) * ratio;
				//this.sliderTop = this.sliderTopOffset + remaining;
			}
		},
		computed: {
		},
		watch: {
		}
	});
</script>
