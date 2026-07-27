export type Game = {
  id: string;
  slug: string;
  title: string;
  description: string | null;
  price: number;
  releaseDate: string | null;
  coverImageUrl: string | null;
  sellerId: string;
  sellerName: string;
};
