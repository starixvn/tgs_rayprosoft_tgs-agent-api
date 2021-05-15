import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"

const Add = "Add"
const AddTrans = {
    zh: "加钱",
    en: "Add"
}

const Subtract = "Subtract"
const SubtractTrans = {
    zh: "扣钱",
    en: "Deduct"
}

export default {
    Add,
    Subtract
}
