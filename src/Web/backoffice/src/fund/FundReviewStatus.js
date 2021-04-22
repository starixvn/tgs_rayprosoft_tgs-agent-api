import React, { useRef, useEffect } from "react"
import { css } from "@emotion/react"
import { Trans } from "../i18n"

const Review = "Review"
const ReviewTrans = {
    zh: "审核中",
    en: "Review"
}

const Approved = "Approved"
const ApprovedTrans = {
    zh: "已通过",
    en: "Approved"
}

const Rejected = "Rejected"
const RejectedTrans = {
    zh: "已拒绝",
    en: "Rejected"
}

export const FundReviewStatusLabel = ({ value }) => {
    switch (value) {
        case Review:
            return <Trans {...ReviewTrans} />
        case Approved:
            return <Trans {...ApprovedTrans} />
        case Rejected:
            return <Trans {...RejectedTrans} />
        default:
            return value ?? ""
    }
}

export default {
    Review,
    Approved,
    Rejected
}
