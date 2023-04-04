using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UITtooltip.Utils {
        public class TooltipUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
        {
            private bool mouseOver;
            private float mouseOverPerSecFuncTimer;

            public Action MouseOverOnce = null;
            public Action MouseOutOnce = null;
            public Action MouseOverFunc = null;
            public Action MouseOverPerSec = null; 
            public Action MouseChange = null;

            public virtual void OnPointerEnter(PointerEventData eventData) {
                if (MouseOverOnce != null) {
                    MouseOverOnce();
                }
                mouseOver = true;
                mouseOverPerSecFuncTimer = 0f; 
            }

            public virtual void OnPointerExit(PointerEventData eventData) {
                if (MouseOutOnce != null) {
                    MouseOutOnce();
                }
                mouseOver = false;
            }

            public bool IsMouseOver() {
                return mouseOver;
            }

            private void Update() {
                if (mouseOver) {
                    if (MouseOverFunc != null) {
                        MouseOverFunc();
                    }
                    mouseOverPerSecFuncTimer -= Time.unscaledDeltaTime;
                    if (mouseOverPerSecFuncTimer <= 0) {
                        mouseOverPerSecFuncTimer += 1f;
                        if (MouseOverPerSec != null) {
                            MouseOverPerSec();
                        }
                    }
                }
                if (MouseChange != null) MouseChange();
            }
        } 
    }


