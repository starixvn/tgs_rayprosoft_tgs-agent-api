import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
    Dialog,
    useModalState,
    Stack,
    Heading,
    Text,
    Box,
    Set,
    FieldStack
} from "bumbag"
import { useIsDisplayMenuByRoute } from "../../user/context"
import Disclosure from "../../dialog/Disclosure"
import { FormattedNumberDecimal, Trans, useTrans } from "../../i18n"
import {
    CloseButtonFooter,
    ConfirmButtonFooter
} from "../../form/ConfirmButtonFooter"
import { Table, toApiParams, useTable, pageCount } from "../../table"
import { usePlayerVipRebateList } from "./client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import dateTimeColumn from "../../table/column/dateTime"
import {
    numberDecimalColumn,
    numberDecimalPercentageColumn
} from "../../table/column/numberColumn"
import AlertDialog from "../../dialog/Alert"
import { startOfNextDay } from "../../date/startOfNextDay"

export const PlayerRebatDialogDisclosure = ({
    brandId,
    username,
    start,
    end,
    value,
    ...props
}) => {
    let dialog = useModalState()

    return (
        <>
            <Disclosure {...dialog} {...props}>
                <FormattedNumberDecimal value={value} />{" "}
            </Disclosure>
            {dialog.visible && (
                <ReportDialog
                    brandId={brandId}
                    username={username}
                    start={start}
                    end={end}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const ReportDialog = ({ brandId, username, start, end, dialog }) => {
    let { state, setPageIndex, setSortOrder } = useTable({
        brandId,
        username,
        from: start,
        to: end,
        showAll: true
    })

    let { from, to } = state
    let { data, isLoading, error, mutate, deleteCache } =
        usePlayerVipRebateList(username, {
            ...toApiParams(state),
            /*
        sortby: "reportDate",
        */
            from,
            to: startOfNextDay(to)
        })

    let columns = usePlayerVipRebateColumns({ items: data?.items })

    let title = useTrans({
        zh: "返水详情",
        en: "Commission Rebate Detail"
    })

    if (isLoading) {
        return <LoadingBackdrop {...dialog} />
    }

    if (error) {
        return <AlertDialog {...dialog} />
    }

    return (
        <Dialog.Modal
            title={title}
            hideOnClickOutside={false}
            {...dialog}
            showCloseButton
            placement="top"
            showActionButtons={false}
            footer={<CloseButtonFooter dialog={dialog} />}
            maxWidth="90vw"
            maxHeight="90vh"
        >
            <Stack>
                <Table
                    columns={columns}
                    data={data?.items}
                    // pageCount={pageCount(pageSize, data?.totalCount)}
                    setPageIndex={setPageIndex}
                    setSortOrder={setSortOrder}
                    isLoading={isLoading}
                    foot={data?.items?.length > 0}
                />
            </Stack>
        </Dialog.Modal>
    )
}

const usePlayerVipRebateColumns = ({ items = [] }) => {
    return useMemo(
        () => [
            dateTimeColumn({
                accessor: "reportDate",
                dateFormat: "yyyy/MM/dd",
                timeFormat: false,
                Header: <Trans zh="日期" en="Date" />,
                Footer: () => <Trans zh="总计" en="Total" />,
                footerCellProps: {
                    textAlign: "center"
                },
                disableSortBy: true
            }),
            numberDecimalPercentageColumn({
                accessor: "liveDealerRebatePercentage",
                Header: (
                    <Trans
                        zh="真人返水比例"
                        en="Live Dealer Commission Rebate %"
                    />
                ),
                Footer: () => <Box />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "liveDealerTurnover",
                Header: <Trans zh="真人投注金额" en="Live Dealer Turnover" />,
                sumColumn: "liveDealerTurnover",
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "liveDealerAmount",
                Header: (
                    <Trans
                        zh="真人返水金额"
                        en="Live Dealer Commission Rebate Amount"
                    />
                ),
                sumColumn: "liveDealerAmount",
                disableSortBy: true
            }),

            numberDecimalPercentageColumn({
                accessor: "sportsRebatePercentage",
                Header: (
                    <Trans zh="体育返水比例" en="Sports Commission Rebate %" />
                ),
                Footer: () => <Box />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "sportsTurnover",
                Header: <Trans zh="体育投注金额" en="Sports Turnover" />,
                sumColumn: "sportsTurnover",
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "sportsAmount",
                Header: (
                    <Trans
                        zh="体育返水金额"
                        en="Sports Commission Rebate Amount"
                    />
                ),
                sumColumn: "sportsAmount",
                disableSortBy: true
            }),

            numberDecimalPercentageColumn({
                accessor: "slotRebatePercentage",
                Header: (
                    <Trans zh="电子返水比例" en="Slot Commission Rebate %" />
                ),
                Footer: () => <Box />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "slotTurnover",
                Header: <Trans zh="电子投注金额" en="Slot Turnover" />,
                sumColumn: "slotTurnover",
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "slotAmount",
                Header: (
                    <Trans
                        zh="电子返水金额"
                        en="Slot Commission Rebate Amount"
                    />
                ),
                sumColumn: "slotAmount",
                disableSortBy: true
            }),

            numberDecimalPercentageColumn({
                accessor: "cardGameRebatePercentage",
                Header: (
                    <Trans
                        zh="棋牌返水比例"
                        en="Card Game Commission Rebate %"
                    />
                ),
                Footer: () => <Box />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "cardGameTurnover",
                Header: <Trans zh="棋牌投注金额" en="Card Game Turnover" />,
                sumColumn: "cardGameTurnover",
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "cardGameAmount",
                Header: (
                    <Trans
                        zh="棋牌返水金额"
                        en="Card Game Commission Rebate Amount"
                    />
                ),
                sumColumn: "cardGameAmount",
                disableSortBy: true
            }),

            numberDecimalPercentageColumn({
                accessor: "virtualSportsRebatePercentage",
                Header: (
                    <Trans
                        zh="虚体返水比例"
                        en="Virtual Sports Commission Rebate %"
                    />
                ),
                Footer: () => <Box />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "virtualSportsTurnover",
                Header: (
                    <Trans zh="虚体投注金额" en="Virtual Sports Turnover" />
                ),
                sumColumn: "virtualSportsTurnover",
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "virtualSportsAmount",
                Header: (
                    <Trans
                        zh="虚体返水金额"
                        en="Virtual Sports Commission Rebate Amount"
                    />
                ),
                sumColumn: "virtualSportsAmount",
                disableSortBy: true
            }),

            numberDecimalPercentageColumn({
                accessor: "lotteryRebatePercentage",
                Header: (
                    <Trans zh="彩票返水比例" en="Lottery Commission Rebate %" />
                ),
                Footer: () => <Box />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "lotteryTurnover",
                Header: <Trans zh="彩票投注金额" en="Lottery Turnover" />,
                sumColumn: "lotteryTurnover",
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "lotteryAmount",
                Header: (
                    <Trans
                        zh="彩票返水金额"
                        en="Lottery Commission Rebate Amount"
                    />
                ),
                sumColumn: "lotteryAmount",
                disableSortBy: true
            })
        ],
        [JSON.stringify(items)]
    )
}
