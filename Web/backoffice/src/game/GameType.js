import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Locales, Trans, useTrans } from "../i18n"
import { Select } from "../select/Select"

const Sportsbook = "Sports"
const SportsbookTrans = {
  zh: "体育竞赛",
  en: "Sportsbook"
}

const VirtualSports = "VirtualSports"
const VirtualSportsTrans = {
  zh: "虚拟体育",
  en: "Virtual Sports"
}

const ESports = "ESports"
const ESportsTrans = {
  zh: "电竞体育",
  en: "E-sports"
}

const LiveDealer = "LiveDealer"
const LiveDealerTrans = {
  zh: "真人娱乐",
  en: "Live Dealer"
}

const LiveDealerLobby = "LiveDealerLobby"
const LiveDealerLobbyTrans = {
  zh: "真人视讯大厅",
  en: "Live Dealer Lobby"
}

const CardGame = "CardGame"
const CardGameTrans = {
  zh: "棋牌",
  en: "Card Game"
}

const CardGameLobby = "CardGameLobby"
const CardGameLobbyTrans = {
  zh: "棋牌大厅",
  en: "Card Game Lobby"
}

const Fishing = "Fishing"
const FishingTrans = {
  zh: "捕鱼",
  en: "Fishing"
}

const FishingLobby = "FishingLobby"
const FishingLobbyTrans = {
  zh: "捕鱼大厅",
  en: "Fishing Lobby"
}

const Arcade = "Arcade"
const ArcadeTrans = {
  zh: "街机游戏",
  en: "Arcade Game"
}

const ArcadeLobby = "ArcadeLobby"
const ArcadeLobbyTrans = {
  zh: "接机游戏大厅",
  en: "Arcade Game Lobby"
}

const Slot = "Slot"
const SlotTrans = {
  zh: "电子游艺",
  en: "Slot Game"
}

const Lottery = "Lottery"
const LotteryTrans = {
  zh: "彩票游戏",
  en: "Lottery"
}

const TableGame = "TableGame"
const TableGameTrans = {
  zh: "桌面游戏",
  en: "Table Game"
}

export const GameType = {
  Sportsbook,
  VirtualSports,
  ESports,
  LiveDealer,
  LiveDealerLobby,
  CardGame,
  CardGameLobby,
  Fishing,
  FishingLobby,
  Arcade,
  ArcadeLobby,
  Slot,
  Lottery,
  TableGame
}

export const GameTypeLabel = ({ value }) => {
  switch (value) {
    case Sportsbook:
      return <Trans {...SportsbookTrans} />

    case VirtualSports:
      return <Trans {...VirtualSportsTrans} />

    case ESports:
      return <Trans {...ESportsTrans} />

    case LiveDealer:
      return <Trans {...LiveDealerTrans} />

    case LiveDealerLobby:
      return <Trans {...LiveDealerLobbyTrans} />

    case CardGame:
      return <Trans {...CardGameTrans} />

    case CardGameLobby:
      return <Trans {...CardGameLobbyTrans} />

    case Fishing:
      return <Trans {...FishingTrans} />

    case FishingLobby:
      return <Trans {...FishingLobbyTrans} />

    case Arcade:
      return <Trans {...ArcadeTrans} />

    case ArcadeLobby:
      return <Trans {...ArcadeLobbyTrans} />

    case Slot:
      return <Trans {...SlotTrans} />

    case Lottery:
      return <Trans {...LotteryTrans} />

    case TableGame:
      return <Trans {...TableGameTrans} />
    default:
      return value
  }
}

export const GameTypeSelect = ({ placeholder, ...props }) => {
  let defaultPlaceholder = useTrans({
    zh: "游戏类型",
    en: "Game Type"
  })
  placeholder = placeholder ?? defaultPlaceholder

  let options = [
    {
      label: useTrans(SportsbookTrans),
      value: Sportsbook
    },
    {
      label: useTrans(VirtualSportsTrans),
      value: VirtualSports
    },
    {
      label: useTrans(ESportsTrans),
      value: ESports
    },
    {
      label: useTrans(LiveDealerTrans),
      value: LiveDealer
    },
    {
      label: useTrans(LiveDealerLobbyTrans),
      value: LiveDealerLobby
    },
    {
      label: useTrans(CardGameTrans),
      value: CardGame
    },
    {
      label: useTrans(CardGameLobbyTrans),
      value: CardGameLobby
    },
    {
      label: useTrans(FishingTrans),
      value: Fishing
    },
    {
      label: useTrans(FishingLobbyTrans),
      value: FishingLobby
    },
    {
      label: useTrans(ArcadeTrans),
      value: Arcade
    },
    {
      label: useTrans(ArcadeLobbyTrans),
      value: ArcadeLobby
    },
    {
      label: useTrans(SlotTrans),
      value: Slot
    },
    {
      label: useTrans(LotteryTrans),
      value: Lottery
    },
    {
      label: useTrans(TableGameTrans),
      value: TableGame
    }
  ]

  return <Select placeholder={placeholder} options={options} {...props} />
}

export default GameType
